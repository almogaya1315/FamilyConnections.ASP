using FamilyConnections.Core.DTO;
using FamilyConnections.Core.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConnections.DAL.Contexts
{
    public class FamConnContext
    {
        private const string AllConnectionsPath = "..\\FamilyConnections.DAL\\Files\\AllConnections.txt";
        private const string AllPersonsPath = "..\\FamilyConnections.DAL\\Files\\AllPersons.txt";

        private List<ConnectionDTO> ReadConnections(List<PersonDTO> allPersons, out List<FlatConnection> connectionsFlat)
        {
            var connectionsJson = File.ReadAllLines(AllConnectionsPath);
            connectionsFlat = connectionsJson.Select(c => JsonConvert.DeserializeObject<FlatConnection>(c)).ToList();
            var connectionsDTO = connectionsFlat.Select(f => new ConnectionDTO(allPersons.Find(p => p.Id == f.TargetId), allPersons.Find(p => p.Id == f.RelatedId), RelationshipInfo.Get(f.RelationshipId)));
            return connectionsDTO.ToList();
        }

        public List<ConnectionDTO> GetConnections(out List<FlatConnection> connectionsFlat, List<PersonDTO> allPersons = null)
        {
            if (allPersons == null) allPersons = GetPersons();
            //var connectionsJson = File.ReadAllLines(AllConnectionsPath);
            //var connectionsFlat = connectionsJson.Select(c => JsonConvert.DeserializeObject<FlatConnection>(c)).ToList();
            var connections = ReadConnections(allPersons, out connectionsFlat);
            return connections.ToList();

            //var AllPersons = GetPersons();
            //var person1 = AllPersons[0];
            //var person2 = AllPersons[1];
            //var person3 = AllPersons[2];
            //var person4 = AllPersons[3];

            //var AllConnections = new List<ConnectionDTO>();
            //var conn = new ConnectionDTO(person1, person2, eRel.Wife);
            //File.AppendAllLines(AllConnectionsPath, new List<string> { JsonConvert.SerializeObject(conn.Flat) });
            ////AllConnections.Add(new ConnectionDTO(person1, person2, eRel.Wife));
            //conn = new ConnectionDTO(person1, person3, eRel.Daughter);
            //File.AppendAllLines(AllConnectionsPath, new List<string> { JsonConvert.SerializeObject(conn.Flat) });
            ////AllConnections.Add(new ConnectionDTO(person1, person3, eRel.Daughter));
            //conn = new ConnectionDTO(person1, person4, eRel.Daughter);
            //File.AppendAllLines(AllConnectionsPath, new List<string> { JsonConvert.SerializeObject(conn.Flat) });
            ////AllConnections.Add(new ConnectionDTO(person1, person4, eRel.Daughter));

            //conn = new ConnectionDTO(person2, person1, eRel.Husband);
            //File.AppendAllLines(AllConnectionsPath, new List<string> { JsonConvert.SerializeObject(conn.Flat) });
            ////AllConnections.Add(new ConnectionDTO(person2, person1, eRel.Husband));
            //conn = new ConnectionDTO(person2, person3, eRel.Daughter);
            //File.AppendAllLines(AllConnectionsPath, new List<string> { JsonConvert.SerializeObject(conn.Flat) });
            ////AllConnections.Add(new ConnectionDTO(person2, person3, eRel.Daughter));
            //conn = new ConnectionDTO(person2, person4, eRel.Daughter);
            //File.AppendAllLines(AllConnectionsPath, new List<string> { JsonConvert.SerializeObject(conn.Flat) });
            ////AllConnections.Add(new ConnectionDTO(person2, person4, eRel.Daughter));

            //conn = new ConnectionDTO(person3, person1, eRel.Father);
            //File.AppendAllLines(AllConnectionsPath, new List<string> { JsonConvert.SerializeObject(conn.Flat) });
            ////AllConnections.Add(new ConnectionDTO(person3, person1, eRel.Father));
            //conn = new ConnectionDTO(person3, person2, eRel.Mother);
            //File.AppendAllLines(AllConnectionsPath, new List<string> { JsonConvert.SerializeObject(conn.Flat) });
            ////AllConnections.Add(new ConnectionDTO(person3, person2, eRel.Mother));
            //conn = new ConnectionDTO(person3, person4, eRel.Sister);
            //File.AppendAllLines(AllConnectionsPath, new List<string> { JsonConvert.SerializeObject(conn.Flat) });
            ////AllConnections.Add(new ConnectionDTO(person3, person4, eRel.Sister));

            //conn = new ConnectionDTO(person4, person1, eRel.Father);
            //File.AppendAllLines(AllConnectionsPath, new List<string> { JsonConvert.SerializeObject(conn.Flat) });
            ////AllConnections.Add(new ConnectionDTO(person4, person1, eRel.Father));
            //conn = new ConnectionDTO(person4, person2, eRel.Mother);
            //File.AppendAllLines(AllConnectionsPath, new List<string> { JsonConvert.SerializeObject(conn.Flat) });
            ////AllConnections.Add(new ConnectionDTO(person4, person2, eRel.Mother));
            //conn = new ConnectionDTO(person4, person3, eRel.Sister);
            //File.AppendAllLines(AllConnectionsPath, new List<string> { JsonConvert.SerializeObject(conn.Flat) });
            ////AllConnections.Add(new ConnectionDTO(person4, person3, eRel.Sister));

            //return AllConnections;
        }

        public List<PersonDTO> GetPersons()
        {
            var personsJson = File.ReadAllLines(AllPersonsPath);
            var persons = personsJson.Select(p => JsonConvert.DeserializeObject<PersonDTO>(p)).ToList();

            var conns = ReadConnections(persons, out List<FlatConnection> connectionsFlat);
            foreach (var person in persons)
            {
                //var personConns = conns.Where(c => c.TargetPerson.Id == 1);
                //person.Connections = personConns.ToDictionary(k => k.RelatedPerson, v => v.Relationship.Type.Value);
                person.FlatConnections = connectionsFlat.Where(f => f.TargetId == person.Id).ToList();
            }
            return persons;

            //var person1 = new PersonDTO
            //{
            //    Id = 1,
            //    FullName = "Lior Matsliah",
            //    DateOfBirth = new DateTime(1985, 5, 23),
            //    PlaceOfBirth = "Israel",
            //};
            //File.AppendAllLines(AllPersonsPath, new List<string> { JsonConvert.SerializeObject(person1) });

            //var person2 = new PersonDTO
            //{
            //    Id = 2,
            //    FullName = "Keren Matsliah",
            //    DateOfBirth = new DateTime(1984, 2, 5),
            //    PlaceOfBirth = "Israel",
            //};
            //File.AppendAllLines(AllPersonsPath, new List<string> { JsonConvert.SerializeObject(person2) });

            //var person3 = new PersonDTO
            //{
            //    Id = 3,
            //    FullName = "Gaya Matsliah",
            //    DateOfBirth = new DateTime(2013, 6, 6),
            //    PlaceOfBirth = "Israel",
            //};
            //File.AppendAllLines(AllPersonsPath, new List<string> { JsonConvert.SerializeObject(person3) });

            //var person4 = new PersonDTO
            //{
            //    Id = 4,
            //    FullName = "Almog Matsliah",
            //    DateOfBirth = new DateTime(2015, 3, 26),
            //    PlaceOfBirth = "Israel",
            //};
            //File.AppendAllLines(AllPersonsPath, new List<string> { JsonConvert.SerializeObject(person4) });

            //return new List<PersonDTO> { person1, person2, person3, person4 };
        }

        public void AddPerson(PersonDTO newPerson)
        {
            File.AppendAllLines(AllPersonsPath, new List<string> { JsonConvert.SerializeObject(newPerson) });

            //var persons = GetPersons();
            //persons.Add(newPerson);
            //File.WriteAllText(AllPersonsPath, string.Empty);
            //foreach (var person in persons)
            //    File.AppendAllLines(AllPersonsPath, new List<string> { JsonConvert.SerializeObject(person) });
        }

        public void AddConnections(ConnectionDTO[] newConnections)
        {
            foreach (var conn in newConnections)
                File.AppendAllLines(AllConnectionsPath, new List<string> { JsonConvert.SerializeObject(conn.Flat) });
        }
    }
}
