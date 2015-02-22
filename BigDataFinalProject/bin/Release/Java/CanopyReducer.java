package CanopyKMeans;

import java.io.IOException;
import java.util.ArrayList;
import org.apache.hadoop.mapreduce.Reducer;

public class CanopyReducer extends
		Reducer<Vector, Vector, Vector, Vector> {
	
	ArrayList<Vector> centers = new ArrayList<Vector>();

	@Override
	protected void reduce(Vector key, Iterable<Vector> values,
			Context context) throws IOException, InterruptedException {
		if (centers.size() == 0) {
			centers.add(new Vector(key));
			context.write(new Vector(key), new Vector(key));
		}
		else {
			boolean isInCluster = false;
			
			for (Vector currCenter : centers) {
				if (currCenter.measureDistance(key) <= Integer.parseInt(context.getConfiguration().get("t2"))) {
					isInCluster = true;
					break;
				}
			}
			
			if (!isInCluster) {
				centers.add(new Vector(key));
				context.write(new Vector(key), new Vector(key));
			}
		}
	}
}
