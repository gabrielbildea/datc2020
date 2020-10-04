using System;
using System.Collections.Generic;

namespace students_api
{
    public static class StudentRepo
    {
    public static List<Student> StudentsList = new List<Student>(){
	    new Student() {Id=1, Nume="Gabriel", Prenume="Bildea", Facultate="AC", An_de_studiu = 1},
	    new Student() {Id=2, Nume="Alexandru", Prenume="Mihai", Facultate="ETC", An_de_studiu = 3},
	    new Student() {Id=3, Nume="Ana", Prenume="Popescu", Facultate="Arhitectura", An_de_studiu = 2},
	    new Student() {Id=4, Nume="Cristina", Prenume="Maria", Facultate="Mecanica", An_de_studiu = 1}
    };
    }
}