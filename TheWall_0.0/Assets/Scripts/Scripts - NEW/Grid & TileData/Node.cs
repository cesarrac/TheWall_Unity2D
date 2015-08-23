using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Node  {

		public List<Node> neighbors;
		public int x;
		public int y;
		public int nodeID;
		
		public Node(){
			neighbors = new List<Node>();
		}
		public float DistanceTo(Node n){
			return Vector2.Distance (
				new Vector2(x, y),
				new Vector2(n.x, n.y)
				);
		}
		
		//		public int IComparable.CompareTo(Node n){
		//			return this.nodeID.CompareTo (n.nodeID);
		//		}

}
