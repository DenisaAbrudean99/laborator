using System.Collections.Generic;
using Models;

namespace Repositories
{
	public static class StudentRepo
	{
		public static List<Student> Students = new List<Student>()
		{
			new Student() { Id = 1, Nume = "Pop", Prenume = "Anca", Facultate = "AC", An = 3 },
			new Student() { Id = 2, Nume = "Achimet", Prenume = "Darius", Facultate = "ETC", An = 4 }
		};

	}


}

