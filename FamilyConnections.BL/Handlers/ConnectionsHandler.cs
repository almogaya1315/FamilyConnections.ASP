using FamilyConnections.Core.DTO;
using FamilyConnections.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConnections.BL.Handlers
{
    public static class ConnectionsHandler
    {
        private static ConnectionDTO _personConnection;
        private static ConnectionDTO _relatedConnection;
        private static eRel? _newRel;
        //private static bool _relFound;
        //private static bool _personRelFound;
        //private static bool _relatedRelFound;
        //private static eDTO _checkingPerson;
        private static eStatus _status;

        public static void Init(this PersonDTO personDTO, ConnectionDTO personConnection, ConnectionDTO relatedConnection)
        {
            _personConnection = personConnection;
            _relatedConnection = relatedConnection;
            //_checkingPerson = eDTO.Person;
            //_status = eStatus.Init;
        }
        public static eRel GetRel(this PersonDTO personDTO)
        {
            return _newRel.Value;
        }

        #region Has methods - relation checkers
        public static PersonDTO HasSibling(this PersonDTO personDTO)
        {
            var conn = _status == eStatus.Init ? _personConnection : _relatedConnection; 
            //_relFound = conn.Relationship.Type == eRel.Sister || conn.Relationship.Type == eRel.Brother;
            _status = eStatus.RelFound;
            return personDTO;
        }
        public static PersonDTO HasSpouse(this PersonDTO personDTO)
        {
            var conn = _status == eStatus.Init ? _personConnection : _relatedConnection;
            //_relFound = conn.Relationship.Type == eRel.Wife || conn.Relationship.Type == eRel.Husband;
            _status = eStatus.RelFound;
            return personDTO;
        }
        public static PersonDTO HasChild(this PersonDTO personDTO)
        {


            return personDTO;
        }
        public static PersonDTO HasParent(this PersonDTO personDTO)
        {


            return personDTO;
        }
        public static PersonDTO HasXXX(this PersonDTO personDTO)
        {


            return personDTO;
        }
        #endregion

        #region Is methods - relation setters
        public static PersonDTO IsSiblingInLaw(this PersonDTO personDTO)
        {
            _newRel = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.SisterInLaw : eRel.BrotherInLaw;
            _status = eStatus.NewRelFound;
            return personDTO;
        }
        public static PersonDTO IsSiblingChild(this PersonDTO personDTO)
        {
            _newRel = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Niece : eRel.Nephew;
            _status = eStatus.NewRelFound;
            return personDTO;
        }
        public static PersonDTO IsParent(this PersonDTO personDTO)
        {
            _newRel = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Mother : eRel.Father;
            _status = eStatus.NewRelFound;
            return personDTO;
        }
        public static PersonDTO IsSibling(this PersonDTO personDTO)
        {
            _newRel = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Sister : eRel.Brother;
            _status = eStatus.NewRelFound;
            return personDTO;
        }
        public static PersonDTO IsXXX(this PersonDTO personDTO)
        {

            return personDTO;
        }
        #endregion

        #region pronouns - status modifiers
        public static PersonDTO That(this PersonDTO personDTO)
        {
            //if (_checkingPerson == eDTO.Person)
            //{
            //    _personRelFound = _relFound;
            //    _relFound = _relatedRelFound = false;
            //    _status = eStatus.PersonRelFound;
            //}
            //else
            //{
            //    _relatedRelFound = _relFound;
            //    _relFound = _personRelFound = false;
            //    _status = eStatus.RelatedRelFound;
            //}

            //_checkingPerson = eDTO.Related;

            return personDTO;
        }
        public static PersonDTO Then(this PersonDTO personDTO)
        {
            //if (_relFound)
            //{

            //}
            //else
            //{

            //}

            return personDTO;
        }
        public static PersonDTO Or(this PersonDTO personDTO)
        {


            return personDTO;
        }
        #endregion
    }

    public enum eDTO
    {
        Person,
        Related
    }

    public enum eStatus
    {
        Init,
        RelFound,
        PersonRelFound,
        RelatedRelFound,
        NewRelFound
    }
}
