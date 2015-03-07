package CanopyKMeans;

import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.conf.Configured;
import org.apache.hadoop.fs.FileSystem;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.mapreduce.Job;
import org.apache.hadoop.mapreduce.lib.input.SequenceFileInputFormat;
import org.apache.hadoop.mapreduce.lib.output.FileOutputFormat;
import org.apache.hadoop.mapreduce.lib.output.SequenceFileOutputFormat;
import org.apache.hadoop.util.Tool;
import org.apache.hadoop.util.ToolRunner;

public class CanopyKMeansJob extends Configured implements Tool {
	
	@Override
	public int run(String[] args) throws Exception 
	{
		String basePath = args[0];
		Configuration conf = new Configuration();

		conf.set("k", args[1]);
		conf.set("t1", args[2]);
		conf.set("t2", args[3]);
		conf.set("total.stocks", args[4]);
		
		Path vectors = new Path(basePath + "/input");
		Path canopyCenters = new Path(basePath + "/canopycenters");
		conf.set("canopy.centers.path", canopyCenters.toString() + "/part-r-00000");
		Path kMeansCentroids = new Path(basePath + "/kmeanscentroids");
		conf.set("kmeans.centroids.path", kMeansCentroids.toString() + "/part-r-00000");
		Path out = new Path(basePath + "/clusters");

		FileSystem fs = FileSystem.get(conf);
		
		if (fs.exists(canopyCenters))
			fs.delete(canopyCenters, true);
		
		if (fs.exists(kMeansCentroids))
			fs.delete(kMeansCentroids, true);
		
		if (fs.exists(out))
			fs.delete(out, true);
		
		Job job = new Job(conf);
		job.setJobName("Canopy Centers");
		job.setJarByClass(CanopyKMeansJob.class);

		job.setMapperClass(CanopyMapper.class);
		job.setReducerClass(CanopyReducer.class);

		SequenceFileInputFormat.addInputPath(job, vectors);
		SequenceFileOutputFormat.setOutputPath(job, canopyCenters);
		
		job.setOutputFormatClass(SequenceFileOutputFormat.class);
		
		job.setOutputKeyClass(Vector.class);
		job.setOutputValueClass(Vector.class);

		job.setNumReduceTasks(1);
		
		job.waitForCompletion(true);		
		
		job = new Job(conf);
		job.setJobName("KMeans Centers");
		job.setJarByClass(CanopyKMeansJob.class);

		job.setMapperClass(KMeansCentersMapper.class);
		job.setReducerClass(KMeansCentersReducer.class);

		SequenceFileInputFormat.addInputPath(job, vectors);
		SequenceFileOutputFormat.setOutputPath(job, kMeansCentroids);
		
		job.setOutputFormatClass(SequenceFileOutputFormat.class);
		
		job.setOutputKeyClass(Vector.class);
		job.setOutputValueClass(Vector.class);

		job.setNumReduceTasks(1);
		
		job.waitForCompletion(true);
		
		int iteration = 1;
		int counter = 0;
		
		do {
			conf.set("iteration", iteration + "");

			if (iteration != 1) {
				conf.set("kmeans.centroids.path", 
						 basePath + "/depth_" + (iteration - 1) + "/part-r-00000");
			}
			
			out = new Path(basePath + "/depth_" + iteration);
			
			if (fs.exists(out))
				fs.delete(out, true);
			
			job = new Job(conf);
			job.setJobName("KMeans Clustering " + iteration);
			job.setJarByClass(CanopyKMeansJob.class);

			job.setMapperClass(KMeansMapper.class);
			job.setReducerClass(KMeansReducer.class);
			job.setJarByClass(KMeansMapper.class);

			SequenceFileInputFormat.addInputPath(job, vectors);
			SequenceFileOutputFormat.setOutputPath(job, out);

			job.setOutputFormatClass(SequenceFileOutputFormat.class);
			
			job.setMapOutputKeyClass(Centroid.class);
			job.setMapOutputValueClass(Stock.class);
			
			job.setOutputKeyClass(Vector.class);
			job.setOutputValueClass(Vector.class);

			job.waitForCompletion(true);
			
			iteration++;
			counter = 
				(int)job.getCounters().findCounter(KMeansReducer.Counter.CONVERGED).getValue();
		} while (counter > 0);
		
		conf.set("kmeans.centroids.path", 
					 basePath + "/depth_" + (iteration - 1) + "/part-r-00000");
		out = new Path(basePath + "/output");
		
		if (fs.exists(out))
			fs.delete(out, true);
		
		job = new Job(conf);
		job.setJobName("KMeans Output");
		job.setJarByClass(CanopyKMeansJob.class);

		job.setMapperClass(KMeansMapper.class);
		job.setReducerClass(KMeansFinalReducer.class);
		job.setJarByClass(KMeansMapper.class);

		SequenceFileInputFormat.addInputPath(job, vectors);
		FileOutputFormat.setOutputPath(job, out);
		
		job.setMapOutputKeyClass(Centroid.class);
		job.setMapOutputValueClass(Stock.class);
		
		job.setOutputKeyClass(Vector.class);
		job.setOutputValueClass(Stock.class);

		job.waitForCompletion(true);
		
		boolean success = job.waitForCompletion(true);
		return success ? 0 : 1;
	}

	public static void main(String[] args) throws Exception {
		CanopyKMeansJob driver = new CanopyKMeansJob();
		int exitCode = ToolRunner.run(driver, args);
		System.exit(exitCode);
	}
}