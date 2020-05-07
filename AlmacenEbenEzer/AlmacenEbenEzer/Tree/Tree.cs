using AlmacenEbenEzer.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace AlmacenEbenEzer.Tree
{
	public class Tree<T> where T : IComparable, IFixedSizeText
	{
		internal int Order { get; set; }
		internal int Root { get; set; }
		internal string Path { get; set; }
		internal int LastPosition { get; set; }
		internal FileStream File { get; set; }

		private ICreateFixedSizeText<T> createFixedSizeText = null;

		/// <summary>
		/// To create a new file. Creates a Header and the Root node
		/// </summary>
		/// <param name="Order"></param>
		/// <param name="Path"></param>
		public Tree(int Order, string Path, ICreateFixedSizeText<T> createFixedSizeText)
		{
			this.createFixedSizeText = createFixedSizeText;
			this.Order = Order;
			this.Path = Path;
			Node<T> root = CreateNode(Order);
			Header e = CreateHeader(Order);
			this.LastPosition = e.NextPosition;

			using (var fs = new FileStream(Path, FileMode.OpenOrCreate))
			{
				fs.Write(ByteGenerator.ConvertToBytes(e.ToFixedSizeString()), 0, e.FixedSizeText);
				fs.Write(ByteGenerator.ConvertToBytes(root.ToFixedSizeString()), 0, root.FixedSizeText());
			}
		}


		/// <summary>
		/// To read an existing file. Reads the Header
		/// </summary>
		/// <param name="Order"></param>
		/// <param name="Path"></param>
		/// <param name="createFixedSizeText"></param>
		public Tree(int Order, string Path, ICreateFixedSizeText<T> createFixedSizeText, int c)
		{
			this.Order = Order;
			this.Path = Path;
			this.createFixedSizeText = createFixedSizeText;

			var buffer = new byte[Header.FixedSize];
			using (var fs = new FileStream(Path, FileMode.OpenOrCreate))
			{
				fs.Seek(0, SeekOrigin.Begin);
				fs.Read(buffer, 0, Header.FixedSize);
			}

			var HeaderString = ByteGenerator.ConvertToString(buffer);
			var values = HeaderString.Split(Util.Separator);

			this.Root = Convert.ToInt16(values[0]);
			this.Order = Convert.ToInt16(values[1]);
			this.LastPosition = Convert.ToInt16(values[2]);


		}

		#region Iniciadores
		private void WriteHeader()
		{
			Header header = new Header
			{
				Root = this.Root,
				NextPosition = this.LastPosition,
				Order = this.Order
			};

			using (var fs = new FileStream(this.Path, FileMode.OpenOrCreate))
			{
				fs.Seek(0, SeekOrigin.Begin);
				fs.Write(ByteGenerator.ConvertToBytes(header.ToFixedSizeString()), 0, header.FixedSizeText);
			}
		}

		private Header CreateHeader(int Order)
		{
			Header e = new Header
			{
				Order = Order,
				Root = 1,
				NextPosition = 2
			};
			return e;
		}

		private Node<T> CreateNode(int Order)
		{
			Node<T> node = new Node<T>
			{
				Order = Order,
				Father = Util.NullPointer, // es la raiz actual
				ID = 1
			};

			node.Data = new List<T>();
			node.Children = new List<int>();

			for (int i = 0; i < (4 * (Order - 1)) / 3 + 1; i++)
			{
				node.Children.Add(Util.NullPointer);
			}

			for (int i = 0; i < (4 * (Order - 1)) / 3; i++)
			{
				node.Data.Add(this.createFixedSizeText.CreateNull());
			}

			this.Root = node.ID;
			this.LastPosition++;

			return node;
		}
		#endregion

		#region insert

		public void Add(T data)
		{
			if (data == null)
			{
				throw new ArgumentException("El valor es nulo");
			}

			Insert(this.Root, data);
		}

		private void Insert(int PosicionActual, T data)
		{
			Node<T> node = new Node<T>();

			node = node.ReadNode(this.Path, this.Order, this.Root, PosicionActual, this.createFixedSizeText);

			if (node.PositionInNode(data) != -1)
				throw new ArgumentException("El dato ya está incluido en el nodo");

			if (node.IsLeaf)
			{
				UpData(node, data, Util.NullPointer);
				WriteHeader();
			}
			else
			{
				Insert(node.Children[node.PositionInNode(data)], data);
			}
		}

		private void UpData(Node<T> node, T data, int Right)
		{
			if (node.Full && node.Father != Util.NullPointer)
			{
				Node<T> nFather = new Node<T>();
				nFather.ID = node.Father;
				nFather.ReadNode(this.Path, this.Order, this.Root, node.Father, createFixedSizeText);

				int position = 0;
				for (int i = 0; i < nFather.Children.Count(); i++)
				{
					if (nFather.Children[i] == node.ID)
					{
						position = i;
						break;
					}
				}

				Node<T> tempNode = new Node<T>();
				// del nodo a la derecha
				if (nFather.Children[position + 1] != Util.NullPointer)
				{
					tempNode.ReadNode(this.Path, this.Order, this.Root, nFather.Children[position + 1], createFixedSizeText);
					if (!tempNode.Full)
					{
						node.Data.Add(data);    // se inserta en el nodo que es
						node.Data.Sort();
						T tempData = node.Data[node.Data.Count() - 1]; // el dato que va a subir al padre
						node.Data.Remove(tempData); // remuevo el dato porque ya lo guarde
						node.WriteNodeOnDisk(this.Path);

						T tempData2 = nFather.Data[position];// dato del padre que va a bajar al hijo derecho
						nFather.Data.Insert(position, tempData2); // se inserta el dato en el padre en la posición correcta

						tempNode.Data.Insert(0, data); // se inserta al inicio de la lista el dato que baje del padre

						tempNode.WriteNodeOnDisk(this.Path); //guardo los cambios

						return;
					}
				}
				// del nodo a la izquierda
				if (position > 0)
				{
					tempNode.ReadNode(this.Path, this.Order, this.Root, nFather.Children[position - 1], createFixedSizeText);
					if (!tempNode.Full)
					{
						node.Data.Add(data);    // se inserta en el nodo que es
						node.Data.Sort();
						T tempData = node.Data[0]; // el dato que va a subir al padre
						node.Data.Remove(tempData); // remuevo el dato porque ya lo guarde
						node.WriteNodeOnDisk(this.Path);

						T tempData2 = nFather.Data[position - 1];// dato del padre que va a bajar al hijo izquierdo
						nFather.Data.Insert(position - 1, tempData2); // se inserta el dato en el padre en la posición correcta

						tempNode.Data.Add(data); // se inserta al inicio de la lista el dato que baje del padre

						tempNode.WriteNodeOnDisk(this.Path); //guardo los cambios

						return;
					}
				}

				// hay que unir los hermanos con el padre y separarlos
				List<T> SuperNode = new List<T>();
				foreach (var item in node.Data)
				{
					SuperNode.Add(item); // añado todos los items del nodo actual
				}

				if (nFather.Children[position + 1] != Util.NullPointer) // si es por nodo derecho
				{
					SuperNode.Add(nFather.Data[position]);// añado la raíz en común al super nodo
					tempNode.ReadNode(this.Path, this.Order, this.Root, nFather.Children[position + 1], createFixedSizeText);
				}
				else if (position > 0) // si es por nodo izquierdo
				{
					SuperNode.Add(nFather.Data[position - 1]); // añado la raíz en común al super nodo
					tempNode.ReadNode(this.Path, this.Order, this.Root, nFather.Children[position - 1], createFixedSizeText);
				}

				foreach (var item in tempNode.Data) // añado los items del hermano al super nodo
				{
					SuperNode.Add(item);
				}

				SuperNode.Add(data); // añado el dato a insertar y ordeno
				SuperNode.Sort();

				int Min = (2 * (this.Order - 1)) / 3;
				node.Data.Clear();
				tempNode.Data.Clear();
				int count = 0;
				for (int i = 0; i < this.Order - 1; i++)
				{
					// datos al nodo original
					if (count < Min)
					{
						node.Data.Add(SuperNode[i]);
					}
					else
					{
						node.Data.Add(createFixedSizeText.CreateNull());
					}
					count++;
				}

				// se agregan los datos a las posiciones correspondientes en el padre
				nFather.Data.Insert(position, SuperNode[Min]);
				nFather.Data.Insert(position + 1, SuperNode[(Min * 2) + 1]);

				count = 0;
				for (int i = 0; i < this.Order - 1; i++)
				{
					// datos al nodo hermano
					if (count < Min)
					{
						tempNode.Data.Add(SuperNode[i + Min + 1]);
					}
					else
					{
						tempNode.Data.Add(createFixedSizeText.CreateNull());
					}
					count++;
				}
				count = 0;
				Node<T> newNode = null;
				newNode.Father = nFather.ID;
				newNode.ID = LastPosition;
				LastPosition++; // actualizacion de ultima posicion disponible
								// datos al nuevo nodo
				for (int i = 0; i < this.Order - 1; i++)
				{
					if (count < Min)
					{
						newNode.Data.Add(SuperNode[i + (2 * Min) + 2]);
					}
					else
					{
						newNode.Data.Add(createFixedSizeText.CreateNull());
					}
				}
				// hijos del nuevo nodo
				for (int i = 0; i < this.Order; i++)
				{
					newNode.Children.Add(Util.NullPointer);
				}

				// escritura de nodos
				node.WriteNodeOnDisk(this.Path);
				tempNode.WriteNodeOnDisk(this.Path);
				nFather.WriteNodeOnDisk(this.Path);
				newNode.WriteNodeOnDisk(this.Path);
				// como se agregó un nuevo nodo, hay que actualizar el header
				WriteHeader();
			}
			else if (!node.Full)
			{
				node.InsertData(data);
				node.WriteNodeOnDisk(this.Path);
				return;
			}

			Node<T> NewNode = new Node<T>(this.Order, this.LastPosition, node.Father, createFixedSizeText);
			this.LastPosition++;

			T ToUpdata = createFixedSizeText.CreateNull();

			node.SplitNode(data, Right, NewNode, ToUpdata, createFixedSizeText);

			Node<T> NodeChildern = new Node<T>();
			for (int i = 0; i < NewNode.Children.Count; i++)
			{
				if (NewNode.Children[i] != Util.NullPointer)
				{
					NodeChildern = NodeChildern.ReadNode(this.Path, this.Order, this.Root, NewNode.Children[i], createFixedSizeText);
					NodeChildern.Father = NewNode.ID;
					NodeChildern.WriteNodeOnDisk(Path);
				}
				else
				{
					break;
				}
			}

			if (node.Father == Util.NullPointer)
			{
				Node<T> newRoot = new Node<T>(this.Order, this.LastPosition, Util.NullPointer, createFixedSizeText);
				this.LastPosition++;

				newRoot.Children[0] = node.ID;
				newRoot.Children[1] = NewNode.ID;
				newRoot.InsertData(data, NewNode.ID);

				node.Father = newRoot.ID;
				newRoot.Father = Util.NullPointer;
				NewNode.Father = newRoot.ID;
				this.Root = newRoot.ID;

				newRoot.WriteNodeOnDisk(this.Path);
				node.WriteNodeOnDisk(this.Path);
				NewNode.WriteNodeOnDisk(this.Path);
			}
			else
			{
				node.WriteNodeOnDisk(this.Path);
				NewNode.WriteNodeOnDisk(this.Path);

				Node<T> Father = new Node<T>();
				Father.ReadNode(this.Path, this.Order, this.Root, node.Father, createFixedSizeText);
				UpData(Father, data, NewNode.ID);
			}
		}

		#endregion

		public void Delete()
		{

		}

		private Node<T> Obtain(int ActualPosition, out int position, T data)
		{
			Node<T> nActual = new Node<T>();
			nActual.ReadNode(this.Path, this.Order, this.Root, ActualPosition, this.createFixedSizeText);
			position = nActual.PositionInNode(data);

			if (position != -1)
			{
				return nActual;
			}
			else
			{
				if (nActual.IsLeaf)
				{
					return null;
				}
				else
				{
					int AproxPosition = nActual.AproxPosition(data);
					return Obtain(nActual.Children[AproxPosition], out position, data);
				}
			}
		}

		public T Obtain(T data)
		{
			int position = -1;
			Node<T> nObtained = Obtain(this.Root, out position, data);

			if (nObtained == null)
			{
				throw new ArgumentException("El dato no está en el árbol");
			}
			else
			{
				return nObtained.Data[position];
			}
		}

		public bool Contains(T data)
		{
			int position = -1;
			Node<T> nObtained = Obtain(this.Root, out position, data);

			if (nObtained != null)
			{
				return true;
			}

			return false;
		}
	}
}