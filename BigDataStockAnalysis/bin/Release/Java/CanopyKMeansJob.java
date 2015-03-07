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
	public int run(String[] args) throws Exception {
		// Prepare and clean environment
		String basePath = args[0];
		Configuration conf = new Configuration();
		Job job;

		conf.set("k", args[1]);
		conf.set("radios", args[2]);
		conf.set("total.stocks", args[3]);

		Path vectors = new Path(basePath + "/input");
		Path canopyCenters = new Path(basePath + "/canopycenters");
		Path kMeansCentroids = new Path(basePath + "/kmeanscentroids");
		Path out = new Path(basePath + "/clusters");
		
		conf.set("canopy.centers.path", canopyCenters.toString()
				+ "/part-r-00000");
		conf.set("kmeans.centroids.path", kMeansCentroids.toString()
				+ "/part-r-00000");
		

		FileSystem fs = FileSystem.get(conf);

		if (fs.exists(canopyCenters))
			fs.delete(canopyCenters, true);
		if (fs.exists(kMeansCentroids))
			fs.delete(kMeansCentroids, true);
		if (fs.exists(out))
			fs.delete(out, true);

		// Prepare and start job #1 - Canopy Centers
		job = new Job(conf);
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

		// Prepare and start job #2 - KMeans Centers
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

		// Prepare and start job #3 - KMeans Clustering (K iterations)
		int iteration = 1;
		int counter = 0;

		do {
			conf.set("iteration", iteration + "");

			if (iteration != 1) {
				conf.set("kmeans.centroids.path", basePath + "/depth_"
						+ (iteration - 1) + "/part-r-00000");
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
			counter = (int) job.getCounters()
					.findCounter(KMeansReducer.Counter.CONVERGED).getValue();
		} while (counter > 0);

		conf.set("kmeans.centroids.path", basePath + "/depth_"
				+ (iteration - 1) + "/part-r-00000");
		out = new Path(basePath + "/output");

		if (fs.exists(out))
			fs.delete(out, true);

		// Prepare and start job #4 - KMeans Output
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

		return job.waitForCompletion(true) ? 0 : 1;
	}

	public static void main(String[] args) throws Exception {
		int exitCode = ToolRunner.run(new CanopyKMeansJob(), args);
		System.exit(exitCode);
	}
}