using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Incremental.Database;

public class Employee : MonoBehaviour {

	private const double RESEARCH_PAY = 2;
	private const double COURSE_PAY = 5;
	private const double RESEARCH_POINTS_PAY = 1000;
	private const double COURSE_POINTS_PAY = 100;

	public enum field
	{
		Researcher,
		Programmer,
		Engineer
	}

	private Dictionary<field, int> fieldPay = new Dictionary<field, int> ();
	//using this, create an employee profile for the player, so they can share methods
	public bool player{ get; set;}
	public string name{ get; set;}
	// int from 1-100, so if an employee has 100 they can't learn anything more in this 
	// field, they have reached maximum potential, when doing first research or course in a
	// field the number of points they earn is random, some employees will gain 1 point for 
	// learning OS others 3 points, the fewer points the more potential they have in that field
	public Dictionary<int, field> employeeFields = new Dictionary<int,field> ();
	public List<SoftwareProject> courses = new List<SoftwareProject> ();
	public List<Research> completedResearch = new List<Research>();
	//0.00 - 1.00
	public double loyalty{ get; set;}

	//how much you are paying them
	public int actualWages{ get; set;}

	//how much they want to be payed, as determined by how much they know
	public int expectedWages{
		get{
			int researchPoints = 0;
			int coursePoints = 0;
			int pay = 0;
			foreach(Research r in completedResearch){
				researchPoints += r.cost;
			}
			foreach(SoftwareProject s in courses){
				coursePoints += s.pointCost;
			}
			pay += (int)(RESEARCH_PAY *(((double)(researchPoints))/RESEARCH_POINTS_PAY) );
			pay += (int)(COURSE_PAY *(((double)(coursePoints))/COURSE_POINTS_PAY) );
			return pay;
		}
	}

	//how happy they are as they determined by how underpaid they are
	public int happiness{
		get{
			return (int)(((double)(actualWages/expectedWages))*100);
		}
	}

	//how likely they are to betray you at this given moment
	public int security{
		get{
			double catalyst = 1.00 - loyalty;
			return  100 -(int)(catalyst*((double)(100-happiness)));
		}
	}


}
