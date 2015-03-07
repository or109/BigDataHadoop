package CanopyKMeans;

import java.io.DataInput;
import java.io.DataOutput;
import java.io.IOException;
import java.util.ArrayList;

import org.apache.hadoop.io.Text;

public class Stock extends Vector {
	public Text ID;

	public Stock() {
		super();
		
		this.ID = new Text();
	}
	
	public Stock(Stock stock) {
		super(stock);

		this.ID = new Text(stock.ID);
	}

	public Stock(Text txtVector) {
		this.vector = new ArrayList<Double>();
		
		String strVector = txtVector.toString();
		
		String[] strVectorArray =  strVector.split(",");
		
		this.ID = new Text(strVectorArray[0]);
		
		for (int i = 1; i < strVectorArray.length; i++)
			this.vector.add(Double.parseDouble(strVectorArray[i]));
		
		normalizeVector();
	}

	@Override
	public void write(DataOutput out) throws IOException {
		this.ID.write(out);
		
		super.write(out);
	}

	@Override
	public void readFields(DataInput in) throws IOException {
		this.ID.readFields(in);
		
		super.readFields(in);
	}
	
	@Override
	public String toString() {
		return "Stock ID: " + this.ID + " [vector=" + getArrayString() + "]";
	}
	
	protected void normalizeVector() {
		Double max, min, normalizer;
		
		max = getMaxValue();
		min = getMinValue();
		
		normalizer = max - min;
		
		ArrayList<Double> normalizedValues = new ArrayList<Double>();
		
		for(Double currDouble : this.vector) {
			if (normalizer == 0) {
				normalizedValues.add(0d);
			}
			else {
				normalizedValues.add(10 * ((currDouble - min) / normalizer));
			}
		}
		
		this.vector = normalizedValues;
	}
	
	protected Double getMaxValue() {
		Double max = Double.MIN_VALUE;
		
		for(Double currDouble : this.vector)
			if(currDouble > max)
				max = currDouble;
		
		return max;
	}
	
	protected Double getMinValue() {
		Double min = Double.MAX_VALUE;
		
		for(Double currDouble : this.vector)
			if(currDouble < min)
				min = currDouble;
		
		return min;
	}
}
