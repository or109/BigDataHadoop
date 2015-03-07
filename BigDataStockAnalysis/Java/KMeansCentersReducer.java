package CanopyKMeans;

import java.io.IOException;
import java.util.ArrayList;

import org.apache.hadoop.mapreduce.Reducer;

public class KMeansCentersReducer extends
		Reducer<Vector, Vector, Vector, Vector> {

	static long totalK;
	static ArrayList<Centroid> random = new ArrayList<Centroid>();

	@Override
	protected void reduce(Vector key, Iterable<Vector> values, Context context)
			throws IOException, InterruptedException {
		ArrayList<Vector> vectors = new ArrayList<Vector>();

		for (Vector value : values) {
			vectors.add(new Vector(value));
		}

		double currK = Math.round(-0.001d
				+ ((double) vectors.size())
				/ Integer.parseInt(context.getConfiguration().get(
						"total.stocks"))
				* Integer.parseInt(context.getConfiguration().get("k")));

		for (int i = 0; i < currK; i++) {

			if (vectors.size() <= 0)
				break;

			Vector randomVector = vectors.get((int) Math.floor(Math.random()
					* vectors.size()));
			context.write(new Vector(key), new Vector(randomVector));
			vectors.remove(randomVector);

			totalK++;
		}

		while (vectors.size() > 0
				&& random.size() < Integer.parseInt(context.getConfiguration()
						.get("k")) && currK > 0) {

			Vector vct = vectors.get(0);
			random.add(new Centroid(key, vct));

			vectors.remove(vct);
		}
	}

	@Override
	protected void cleanup(Context context) throws IOException,
			InterruptedException {

		while (totalK < Integer.parseInt(context.getConfiguration().get("k"))
				&& random.size() > 0) {

			Centroid randomCentroid = random.get((int) Math.floor(Math.random()
					* random.size()));
			context.write(new Vector(randomCentroid.center), new Vector(
					randomCentroid.centroid));
			random.remove(randomCentroid);

			totalK++;
		}

		super.cleanup(context);
	}
}
