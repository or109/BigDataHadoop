package CanopyKMeans;

import java.io.IOException;
import java.util.ArrayList;
import org.apache.hadoop.io.LongWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Mapper;

public class CanopyMapper extends
		Mapper<LongWritable, Text, Vector, Vector> {

	static ArrayList<Vector> centers = new ArrayList<Vector>();;

	@Override
	protected void map(LongWritable key, Text value, Context context)
			throws IOException, InterruptedException {
		Stock stock = new Stock(value);
		
		if (centers.size() == 0) {			
			centers.add(new Vector(stock));
			context.write(new Vector(stock), new Vector(stock));
		}
		else {
			boolean isInCluster = false;
			
			for (Vector currCenter : centers) {
				if (currCenter.measureDistance(stock) <= Integer.parseInt(context.getConfiguration().get("t2"))) {
					isInCluster = true;
					break;
				}
			}
			
			if (!isInCluster) {
				centers.add(new Vector(stock));
				context.write(new Vector(stock), new Vector(stock));
			}
		}
	}

}
