package CanopyKMeans;

import java.io.DataInput;
import java.io.DataOutput;
import java.io.IOException;

import org.apache.hadoop.io.WritableComparable;

public class Centroid implements WritableComparable<Centroid> {

	public Vector center;
	public Vector centroid;

	public Centroid() {
		super();
		this.center = null;
		this.centroid = null;
	}

	public Centroid(Vector center, Vector centroid) {
		super();
		this.center = new Vector(center);
		this.centroid = new Vector(centroid);
	}

	public Centroid(Centroid centroid) {
		this(centroid.center, centroid.centroid);
	}

	@Override
	public boolean equals(Object obj) {
		if (obj instanceof Centroid) {
			return ((this.center.compareTo(((Centroid) obj).center) == 0) && (this.centroid
					.compareTo(((Centroid) obj).centroid) == 0));
		} else {
			return super.equals(obj);
		}
	}

	@Override
	public void write(DataOutput out) throws IOException {
		center.write(out);
		centroid.write(out);
	}

	@Override
	public void readFields(DataInput in) throws IOException {
		center = new Vector();
		centroid = new Vector();
		center.readFields(in);
		centroid.readFields(in);
	}

	@Override
	public int compareTo(Centroid o) {
		if (center.compareTo(o.center) != 0) {
			return center.compareTo(o.center);
		} else if (centroid.compareTo(o.centroid) != 0) {
			return centroid.compareTo(o.centroid);
		}

		return 0;
	}

	@Override
	public String toString() {
		return center.toString() + "\r\n" + centroid.toString();
	}
}
