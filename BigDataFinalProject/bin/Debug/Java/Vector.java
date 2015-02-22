package CanopyKMeans;

import java.io.DataInput;
import java.io.DataOutput;
import java.io.IOException;
import java.util.ArrayList;
import org.apache.hadoop.io.WritableComparable;

public class Vector implements WritableComparable<Vector> {

	protected ArrayList<Double> vector;

	public Vector() {
		super();
	}

	public Vector(Vector v) {
		super();
		
		this.vector = new ArrayList<Double>();
		
		for (Double currDouble : v.vector)
			this.vector.add(currDouble);
	}
	
	public Vector(double x, double y) {
		super();
		this.vector = new ArrayList<Double>();
		this.vector.add(x);
		this.vector.add(y);
	}
	
	public Vector(ArrayList<Double> vector) {
		super();
		this.vector = new ArrayList<Double>();
		
		for (Double currDouble : vector)
			this.vector.add(currDouble);
	}

	@Override
	public void write(DataOutput out) throws IOException {
		out.writeInt(vector.size());
		for (Double currDouble : this.vector)
			out.writeDouble(currDouble);
	}

	@Override
	public void readFields(DataInput in) throws IOException {
		int size = in.readInt();
		vector = new ArrayList<Double>();
		for (int i = 0; i < size; i++)
			vector.add(in.readDouble());
	}

	@Override
	public int compareTo(Vector o) {

		if(this.vector.size() != o.vector.size()) {
			return this.vector.size() - o.vector.size();
		}
		
		for (int i = 0; i < vector.size(); i++) {
			double c = vector.get(i) - o.vector.get(i);
			if (c!= 0.0d)
			{
				int result = (int)c;
				
				if(result == 0) {
					if(c < 0) { 
						return -1;
					}
					else {
						return 1;
					}
				}
				return (int)c;			
			}		
		}
		return 0;
	}

	public ArrayList<Double> getVector() {
		return vector;
	}

	public void setVector(ArrayList<Double> vector) {
		this.vector = vector;
	}
	
	public void setVectorSize(int size) {
		this.vector = new ArrayList<Double>();
		
		for(int i=0;i<size;i++)
			this.vector.add(0d);
	}
	
	@Override
	public String toString() {
		return getArrayString();
	}
	
	protected String getArrayString() {
		String arrString = "[";
		
		for (Double currDouble : this.vector) {
			
			if(arrString.length() > 1)
				arrString += ", ";		
			
			arrString += currDouble.toString();
		}
		
		arrString += " ]";
		
		return arrString;
	}
	
	public double measureDistance(Vector v) {
		double sum = 0.0;
	    int minSize = vector.size();
	    
	    if(v.vector.size() < minSize)
	    {
	    	minSize = v.vector.size();
	    }
	    
	    for (int i = 0; i < minSize; i++) {
	    	sum = sum + Math.pow(Math.abs(Double.valueOf(vector.get(i)) - Double.valueOf(v.vector.get(i))),2);
	    }
	    
	    return Math.sqrt(sum);
	}
	
	@Override
	public boolean equals(Object obj) {
		if (obj instanceof Vector) {
			return compareTo((Vector)obj) == 0;
		}
		else{
			return super.equals(obj);
		}
	}
	
	public boolean converged(Vector c) {
		return compareTo(c) == 0 ? false : true;
	}
}
