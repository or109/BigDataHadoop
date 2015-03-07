package CanopyKMeans;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import org.apache.hadoop.mapreduce.Reducer;

public class KMeansReducer extends
		Reducer<Centroid, Vector, Vector, Vector> {

	public static enum Counter {
		CONVERGED
	}

	@Override
	protected void reduce(Centroid key, Iterable<Vector> values,
			Context context) throws IOException, InterruptedException {

		Vector newCenter = new Vector();
		
		List<Vector> vectorList = new ArrayList<Vector>();
		int vectorSize = key.centroid.getVector().size();		
		
		newCenter.setVectorSize(vectorSize);
		
		for (Vector value : values) {
			vectorList.add(new Vector(value));
			for (int i = 0; i < value.getVector().size(); i++) {
				Double currDouble = newCenter.getVector().get(i);			
				
				currDouble += value.getVector().get(i);
				
				newCenter.getVector().set(i, currDouble);
			}
		}

		for (int i = 0; i < newCenter.getVector().size(); i++) {
			Double currDouble = newCenter.getVector().get(i);
			
			currDouble = currDouble / vectorList.size();
			
			newCenter.getVector().set(i, currDouble);
		}
		
		context.write(new Vector(key.center), new Vector(newCenter));
		
		if (key.centroid.converged(newCenter))
			context.getCounter(Counter.CONVERGED).increment(1);

	}
}
