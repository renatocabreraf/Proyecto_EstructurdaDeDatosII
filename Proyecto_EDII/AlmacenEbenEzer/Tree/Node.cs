using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AlmacenEbenEzer.Interfaces;


namespace AlmacenEbenEzer.Tree
{
	public class Node<T> where T : IComparable, IFixedSizeText
	{
		internal List<T> Data { get; set; }
		internal List<int> Children { get; set; }
		internal int Father { get; set; }
		internal int ID { get; set; }
		internal int Order { get; set; }
		internal ICreateFixedSizeText<T> createFixedSizeText = null;

		public Node()
		{

		}

		internal Node(int order, int ID, int father, ICreateFixedSizeText<T> createFixedSizeText)
		{
			if (order < 0)
			{
				throw new ArgumentOutOfRangeException("Orden inválido");
			}
			this.Order = order;
			this.ID = ID;
			this.Father = father;
			this.createFixedSizeText = createFixedSizeText;
			ClearNode(createFixedSizeText);
		}

		private void ClearNode(ICreateFixedSizeText<T> createFixedSizeText)
		{
			Children = new List<int>();
			Data = new List<T>();

			if (Father.Equals(Util.NullPointer))
			{
				int max = (4 * (Order - 1)) / 3;
				for (int i = 0; i < max + 1; i++)
				{
					Children.Add(Util.NullPointer);
				}

				for (int i = 0; i < max; i++)
				{
					Data.Add(createFixedSizeText.CreateNull());
				}
			}
			else
			{
				for (int i = 0; i < Order; i++)
				{
					Children.Add(Util.NullPointer);
				}

				for (int i = 0; i < Order - 1; i++)
				{
					Data.Add(createFixedSizeText.CreateNull());
				}
			}
		}

		internal int FixedSize(int Father)
		{
			int InTextSize = 0;

			InTextSize += Util.IntegerSize + 1; // Posición
			InTextSize += Util.IntegerSize + 1; // Padre

			if (Father == Util.NullPointer)
			{
				InTextSize += (Data[0].FixedSize + 1) * ((4 * (Order - 1)) / 3); //Data
				InTextSize += (Util.IntegerSize + 1) * ((4 * (Order - 1)) / 3) + (Util.IntegerSize + 1);    // Children
			}
			else
			{
				InTextSize += (Data[0].FixedSize + 1) * (Order - 1);
				InTextSize += (Util.IntegerSize + 1) * Order;
			}
			InTextSize += 2; // \r\n

			return InTextSize;

		}

		public int FixedSizeText()
		{
			return FixedSize(this.Father);
		}

		private int SeekPosition(int Root)
		{
			if (ID <= Root)
			{
				return Header.FixedSize + ((ID - 1) * FixedSizeText());
			}
			else
			{
				return Header.FixedSize + ((ID - 1) * FixedSizeText()) + FixedSize(Util.NullPointer);
			}
		}

		#region Format
		private string ChildrensFormat(int Order)
		{
			string Children = "";
			int max = (4 * (Order - 1)) / 3;

			if (Father.Equals(Util.NullPointer))
			{
				for (int i = 0; i < max + 1; i++)
				{
					Children = Children + $"{this.Children[i].ToString("0000000000;-000000000")}" + Util.Separator.ToString(); // 10 caracteres + 1
				}
			}
			else
			{
				for (int i = 0; i < Order; i++)
				{
					Children = Children + $"{this.Children[i].ToString("0000000000;-000000000")}" + Util.Separator.ToString(); // 10 caracteres + 1
				}
			}

			return Children;
		}

		private string DataFormat(int Order)
		{
			string values = null;
			int max = (4 * (Order - 1)) / 3;

			if (Father.Equals(Util.NullPointer))
			{
				for (int i = 0; i < max; i++)
				{
					values = values + $"{Data[i].ToFixedSizeString()}" + Util.Separator.ToString(); // FixedSize del T + 1
				}
			}
			else
			{
				for (int i = 0; i < Order - 1; i++)
				{
					values = values + $"{Data[i].ToFixedSizeString()}" + Util.Separator.ToString(); // FixedSize del T + 1
				}
			}

			return values;
		}

		public string ToFixedSizeString()
		{
			string values = DataFormat(this.Order);
			string childrens = ChildrensFormat(this.Order);
			return $"{ID.ToString("0000000000;-000000000")}" + Util.Separator.ToString() + $"{Father.ToString("0000000000;-000000000")}" + Util.Separator.ToString()
				+ values + childrens + "\r\n";
		}
		#endregion

		#region Read n' Write
		internal Node<T> ReadNode(string Path, int Order, int Root, int ID, ICreateFixedSizeText<T> createFixedSizeText)
		{
			int Father = 0;
			if (ID == Root)
			{
				Father = Util.NullPointer;
			}

			Node<T> node = new Node<T>(Order, ID, Father, createFixedSizeText);

			int HeaderSize = Header.FixedSize;
			byte[] buffer;

			if (ID <= Root)
			{

				buffer = new byte[node.FixedSize(node.Father)];
				using (var fs = new FileStream(Path, FileMode.OpenOrCreate))
				{
					fs.Seek((HeaderSize + ((Root - 1) * node.FixedSize(node.Father))), SeekOrigin.Begin);
					fs.Read(buffer, 0, node.FixedSize(node.Father));
				}
			}
			else
			{
				buffer = new byte[node.FixedSize(node.Father)];
				using (var fs = new FileStream(Path, FileMode.OpenOrCreate))
				{
					fs.Seek((HeaderSize + ((Root - 1) * node.FixedSize(node.Father)) + node.FixedSize(Util.NullPointer)), SeekOrigin.Begin);
					fs.Read(buffer, 0, node.FixedSize(node.Father));
				}
			}



			var NodeString = ByteGenerator.ConvertToString(buffer);
			var Values = NodeString.Split(Util.Separator);

			node.Father = Convert.ToInt32(Values[1]);

			//Hijos

			int DataLimit = Order;

			if (node.Father.Equals(Util.NullPointer))
			{
				DataLimit = (4 * (Order - 1)) / 3;
				int j = 0;
				for (int i = 2; i < DataLimit + 2; i++)
				{
					node.Data[j] = createFixedSizeText.Create(Values[i]);
					j++;
				}
				j = 0;
				int StartLimit = node.Data.Count + 2;
				for (int i = StartLimit; i < Values.Length - 1; i++)
				{
					node.Children[j] = Convert.ToInt32(Values[i]);
					j++;
				}
			}
			else
			{
				int j = 0;
				for (int i = 2; i < DataLimit + 1; i++)
				{
					node.Data[j] = createFixedSizeText.Create(Values[i]);
					j++;
				}
				j = 0;
				int StartLimit = node.Data.Count + 2;
				//Valores
				for (int i = StartLimit; i < Values.Length - 1; i++)
				{
					node.Children[i] = Convert.ToInt32(Values[i]);
					j++;
				}
			}


			return node;
		}

		internal void WriteNodeOnDisk(string Path)
		{
			using (var fs = new FileStream(Path, FileMode.Open))
			{
				fs.Seek(SeekPosition(ID), SeekOrigin.Begin);
				fs.Write(ByteGenerator.ConvertToBytes(ToFixedSizeString()), 0, FixedSizeText());
			}
		}

		internal void LimpiarNodo_Disco(string Path, ICreateFixedSizeText<T> createFixedSizeText)
		{
			ClearNode(createFixedSizeText);

			WriteNodeOnDisk(Path);
		}
		#endregion

		internal int AproxPosition(T data)
		{
			int position = Data.Count;
			for (int i = 0; i < Data.Count; i++)
			{
				if ((Data[i].CompareTo(data) < 0) || (Data[i].CompareTo(createFixedSizeText.CreateNull()) == 0))
				{
					position = i; break;
				}
			}
			return position;
		}

		internal int PositionInNode(T data)
		{
			int position = -1;
			for (int i = 0; i < Data.Count; i++)
			{
				if (data.CompareTo(Data[i]) == 0)
				{
					position = i;
					break;
				}
			}
			return position;
		}

		#region Insert data
		internal void InsertData(T data, int Right)
		{
			InsertData(data, Right, true);
		}

		internal void InsertData(T data, int Right, bool ValidateIfFull)
		{
			if (Full && ValidateIfFull)
			{
				throw new ArgumentOutOfRangeException("El nodo está lleno");
			}

			int PositionToInsert = AproxPosition(data);

			// Correr hijos
			for (int i = Children.Count - 1; i > PositionToInsert + 1; i--)
			{
				Children[i] = Children[i - 1];
			}
			Children[PositionToInsert + 1] = Right;

			// Correr datos
			for (int i = Data.Count - 1; i > PositionToInsert; i--)
			{
				Data[i] = Data[i - 1];
			}
			Data[PositionToInsert] = data;
		}

		internal void InsertData(T data)
		{
			InsertData(data, Util.NullPointer);
		}
		#endregion

		#region Delete n' split
		internal void DeleteData(T data, ICreateFixedSizeText<T> createFixedSizeText)
		{
			if (!IsLeaf)
			{
				throw new Exception("Solo pueden eliminarse en nodos hoja");
			}

			int PositionToDelete = PositionInNode(data);

			if (PositionToDelete == -1)
			{
				throw new ArgumentException("El dato no existe en el nodo");
			}

			for (int i = Data.Count - 1; i > PositionToDelete; i--)
			{
				Data[i - 1] = Data[i];
			}

			Data[Data.Count - 1] = createFixedSizeText.CreateNull();
		}

		internal void SplitNode(T data, int Right, Node<T> Node, T ToUpData, ICreateFixedSizeText<T> createFixedSizeText)
		{
			int Middle = 0;
			if (Father.Equals(Util.NullPointer))
			{
				// Incrementar en una posición
				Data.Add(data);
				Children.Add(Util.NullPointer);

				// Agregarlos en orden
				InsertData(data, Right, false);

				// Dato a subir
				Middle = Data.Count / 2;

				ToUpData = Data[Middle];
				Data[Middle] = createFixedSizeText.CreateNull();

				// Llenar datos que suben
				int j = 0;
				for (int i = Middle + 1; i < Data.Count; i++)
				{
					Node.Data[j] = Data[i];
					Data[i] = createFixedSizeText.CreateNull();
					j++;
				}

				// Llenar hijos que suben
				j = 0;
				for (int i = Middle + 1; i < Children.Count; i++)
				{
					Node.Children[j] = Children[i];
					Children[i] = Util.NullPointer;
					j++;
				}

				Data.RemoveAt(Data.Count - 1);
				Children.RemoveAt(Children.Count - 1);
			}
			else
			{

				Data.Add(data);
				Children.Add(Util.NullPointer);

				// Agregarlos en orden
				InsertData(data, Right, false);

				// Dato a subir
				Middle = Data.Count / 2;

				ToUpData = Data[Middle];
				Data[Middle] = createFixedSizeText.CreateNull();

				// Llenar datos que suben
				int j = 0;
				for (int i = Middle + 1; i < Children.Count; i++)
				{
					Node.Data[j] = Data[i];
					Data[i] = createFixedSizeText.CreateNull();
					j++;
				}

				// Llenar hijos que suben
				j = 0;
				for (int i = Middle + 1; i < Children.Count; i++)
				{
					Node.Children[j] = Children[i];
					Children[i] = Util.NullPointer;
					j++;
				}

				Data.RemoveAt(Data.Count - 1);
				Children.RemoveAt(Children.Count - 1);
			}
		}

		#endregion

		internal int CountData
		{
			get
			{
				int i = 0;
				while (i < Data.Count && Data[i].CompareTo(createFixedSizeText.CreateNull()) != 0)
				{
					i++;
				}
				return i;
			}
		}

		internal bool Underflow
		{
			get { return (CountData < (Order / 2) - 1); }
		}

		internal bool Full
		{
			get
			{
				if (this.Father.Equals(Util.NullPointer))
				{
					return (CountData >= (4 * (Order - 1)) / 3);
				}
				return (CountData >= Order - 1);
			}
		}

		internal bool IsLeaf
		{
			get
			{
				bool Leaf = true;
				for (int i = 0; i < Children.Count; i++)
				{
					if (Children[i] != Util.NullPointer)
					{
						Leaf = false;
						break;
					}
				}
				return Leaf;
			}
		}
	}
}