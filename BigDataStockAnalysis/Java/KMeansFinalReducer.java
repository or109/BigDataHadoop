package CanopyKMeans;

import java.io.IOException;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Reducer;

public class KMeansFinalReducer extends Reducer<Centroid, Stock, Text, Text> {

	public static enum Counter {
		CURR_CENTROID
	}

	@Override
	protected void reduce(Centroid key, Iterable<Stock> values, Context context)
			throws IOException, InterruptedException {

		for (Stock value : values) {
			context.write(
					new Text(String.valueOf(context.getCounter(
							Counter.CURR_CENTROID).getValue())), value.ID);
		}

		context.getCounter(Counter.CURR_CENTROID).increment(1);
	}
}
