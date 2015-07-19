using System;
using System.Collections.Generic;

using Faker;

public class Employee : Asset {

	private const double RESEARCH_PAY = 2;
	private const double COURSE_PAY = 5;
	private const double RESEARCH_POINTS_PAY = 1000;
	private const double COURSE_POINTS_PAY = 100;

    // This looks convuluted, but it's required if we want to set this automagically via dapper.
    // Basically this assigns an integer value (quite obviously) which is stored in the database.
    // When this is being serialized (or stored somewhere), you merely need to cast it to int to 
    // retrieve the value: 
    // e.g. int asd = (int)field.Programmer;
	public Dictionary<Field.field,int> fieldPotential = new Dictionary<Field.field, int> ();

    // Generate a full name (with prefix) only if it remains unassigned
    private string _name;
    public override string name {
        get {
            if (string.IsNullOrEmpty(_name)) {
                _name = Faker.Name.FullName(NameFormats.WithPrefix);
            }
            return _name;
        }
        set {
            _name = value;
        }
    }

    // Pick an enumerator at random
    private Field.field? _jobField;
    public Field.field? jobField {
        get {
            if (_jobField == null) {
                Array values = Enum.GetValues(typeof(Field.field));
                Random random = new Random();
                Field.field randomField = (Field.field)values.GetValue(random.Next(values.Length));
                _jobField = randomField;
            }
            return _jobField;
        }
        set {
            _jobField = value;
        }
    }

	private Dictionary<Field.field, int> fieldPay = new Dictionary<Field.field, int> ();
	//using this, create an employee profile for the player, so they can share methods
	public bool player{ get; set;}
	// int from 1-100, so if an employee has 100 they can't learn anything more in this 
	// field, they have reached maximum potential, when doing first research or course in a
	// field the number of points they earn is random, some employees will gain 1 point for 
	// learning OS others 3 points, the fewer points the more potential they have in that field
	public Dictionary<Field.field,double> employeeFields = new Dictionary<Field.field,double> ();
	public List<SoftwareProject> courses = new List<SoftwareProject> ();
	//only need to put in the research at the limit of their undertanding
	public List<Research> completedResearch = new List<Research>();
	public ResearchController employeeResearch = new ResearchController ();
	public SoftwareController employeeSoftware = new SoftwareController();
	//0.00 - 1.00
	public double loyalty{ get; set;}

	//how much you are paying them
	public int actualWages{ get; set;}

	//how much they want to be payed, as determined by how much they know
	public double expectedWages{
		get{
			int researchPoints = 0;
			int coursePoints = 0;
			double pay = 0;
			foreach(Research r in completedResearch){
				researchPoints += r.cost;
			}
			foreach(SoftwareProject s in courses){
				coursePoints += s.pointCost;
			}
			pay += RESEARCH_PAY * researchPoints / RESEARCH_POINTS_PAY;
			pay += COURSE_PAY * coursePoints / COURSE_POINTS_PAY;
			return pay;
		}
	}

	//how happy they are as they determined by how underpaid they are
	public double happiness{
		get{
			return Math.Floor((actualWages / expectedWages) * 100);
		}
	}

	//how likely they are to betray you at this given moment
	public double security{
		get{
			double catalyst = 1.00 - loyalty;
			return  100 -(catalyst * (100-happiness));
		}
	}

	void addDependecies(List<Research> depends){
		if (depends.Count==0) {
			return;
		} else {
			foreach(Research r in depends){
				if(employeeResearch.AllCompleteResearch.ContainsKey(r.ID)){
					return;
				}
				else{
					employeeResearch.AllUncompleteResearch.Remove(r.ID);
					employeeResearch.AllCompleteResearch.Add (r.ID,r);
					addDependecies(r.Dependencies);
				}

			}
		}
	}

	//called when object is first created
	public void completeProfile(){
		foreach (Research r in completedResearch) {
			employeeResearch.AllUncompleteResearch.Remove(r.ID);
			employeeResearch.AllCompleteResearch.Add (r.ID,r);
			addDependecies(r.Dependencies);
		}
		foreach (SoftwareProject s in courses){
			employeeSoftware.AllCompletedCourses.Add(s);
			employeeSoftware.UnstartedSoftware.Remove(s.ID);
		}
		foreach (Field.field item in Enum.GetValues(typeof(Field.field)) ){
			Random rnd = new Random();
			int rand = rnd.Next (99,1001);
			fieldPotential.Add(item,rand);
			employeeFields.Add (item,0.00);
		}
		foreach (SoftwareProject sDone in employeeSoftware.AllCompletedCourses) {
			int catalyst = fieldPotential[sDone.SoftwareField];
			double points = (double)(sDone.pointCost/catalyst);
			employeeFields[sDone.SoftwareField] += points;
			if(employeeFields[sDone.SoftwareField]>100){
				employeeFields[sDone.SoftwareField] = 100;
			}
		}
		foreach (Research rDone in employeeResearch.AllCompleteResearch.Values) {
			int catalyst = fieldPotential[rDone.ResearchField];
			double points = (double)(rDone.cost/catalyst);
			employeeFields[rDone.ResearchField] += points;
			if(employeeFields[rDone.ResearchField]>100){
				employeeFields[rDone.ResearchField] = 100;
			}
		}
	}



}
