package CanopyKMeans;

import java.io.IOException;
import java.util.ArrayList;

import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.fs.FileSystem;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.io.LongWritable;
import org.apache.hadoop.io.SequenceFile;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Mapper;

public class KMeansCentersMapper extends
		Mapper<LongWritable, Text, Vector, Vector> {

	ArrayList<Vector> centers = new ArrayList<Vector>();

	@Override
	protected void setup(Context context) throws IOException,
			InterruptedException {
		super.setup(context);

		Configuration conf = context.getConfiguration();
		Path canopyCenters = new Path(conf.get("canopy.centers.path"));
		FileSystem fs = FileSystem.get(conf);

		@SuppressWarnings("deprecation")
		SequenceFile.Reader reader = new SequenceFile.Reader(fs, canopyCenters,
				conf);
		// org.apache.hadoop.io.ArrayFile.Reader reader = new
		// org.apache.hadoop.io.ArrayFile.Reader(fs, canopyCenters.toString(),
		// conf);
		Vector key = new Vector();
		Vector value = new Vector();

		while (reader.next(key, value)) {
			centers.add(new Vector(key));
		}

		reader.close();
	}

	@Override
	protected void map(LongWritable key, Text value, Context context)
			throws IOException, InterruptedException {
		Stock stock = new Stock(value);

		Vector bestCenter = null;
		double bestDistance = Double.MAX_VALUE;

		for (Vector currCenter : centers) {
			double currDistance = currCenter.measureDistance(stock);

			if ((bestCenter == null) || (currDistance < bestDistance)) {
				bestDistance = currDistance;
				bestCenter = currCenter;
			}
		}

		context.write(new Vector(bestCenter), new Vector(stock));
	}
}
