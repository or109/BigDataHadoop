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

public class KMeansMapper extends
		Mapper<LongWritable, Text, Centroid, Stock> {

	ArrayList<Centroid> centroids = new ArrayList<Centroid>();

	@Override
	protected void setup(Context context) throws IOException,
			InterruptedException {
		super.setup(context);
		
		Configuration conf = context.getConfiguration();
		
		Path centroidsPath = new Path(conf.get("kmeans.centroids.path"));
		
		FileSystem fs = FileSystem.get(conf);
		
		@SuppressWarnings("deprecation")
		SequenceFile.Reader reader = new SequenceFile.Reader(fs, centroidsPath, conf);
		Vector key = new Vector();
		Vector value = new Vector();
		
		while (reader.next(key, value)) {
			centroids.add(new Centroid(key, value));
		}
		
		reader.close();
	}

	@Override
	protected void map(LongWritable key, Text value, Context context)
			throws IOException, InterruptedException {
		Stock stock = new Stock(value);
		
		Vector nearestCenter = null;
		double canopyDistance = Double.MAX_VALUE;
		Centroid nearestCentroid = null;
		double centroidDistance = Double.MAX_VALUE;
		
		for (Centroid currCentroid : centroids) {
			double dist = currCentroid.center.measureDistance(stock);
			
			if (nearestCenter == null || canopyDistance > dist) {
				nearestCenter = currCentroid.center;
				canopyDistance = dist;
				
				nearestCentroid = null;
				centroidDistance = Double.MAX_VALUE;
			}
			
			if (currCentroid.center.equals(nearestCenter)) {				
				dist = currCentroid.centroid.measureDistance(stock);
				
				if (nearestCentroid == null || centroidDistance > dist) {
					nearestCentroid = currCentroid;
					centroidDistance = dist;
				}
			}
		}
		
		context.write(new Centroid(nearestCentroid), new Stock(stock));
	}

}
