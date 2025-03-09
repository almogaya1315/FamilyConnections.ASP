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
        private static List<ConnectionDTO> _allConnections;

        #region Operation methods
        public static void InitConnection(ConnectionDTO personConnection, ConnectionDTO relatedConnection, IEnumerable<ConnectionDTO> allConnections)
        {
            _personConnection = personConnection;
            _relatedConnection = relatedConnection;
            _allConnections = allConnections.ToList();
        }
        public static void ConnectionBetween(PersonDTO person, PersonDTO relatedPerson, eRel relation,
            ref List<ConnectionDTO> newConnections, eRel? possibleComplexRel, ref List<ConnectionDTO> possibleComplex_debug, bool opposite = false)
        {
            if (relation == eRel.FarRel) return;

            if (opposite)
            {
                relation = RelationshipInfo.Opposite(relation, relatedPerson.Gender.Value);
                if (possibleComplexRel.HasValue) possibleComplexRel = RelationshipInfo.Opposite(possibleComplexRel.Value, relatedPerson.Gender.Value);
            }

            if (!ConnExists(person, relatedPerson, relation, ref newConnections))
            {
                var newConn = new ConnectionDTO(person, relatedPerson, relation, possibleComplexRel);
                if (possibleComplexRel.HasValue) possibleComplex_debug.Add(newConn);
                newConnections.Add(newConn);
                person.AddConnection(newConn);
            }

            if (!opposite)
            {
                ConnectionBetween(relatedPerson, person, relation, ref newConnections, possibleComplexRel, ref possibleComplex_debug, opposite: true);
            }
        }
        public static eRel FindRelation(out eRel? possibleComplexRel)
        {
            var relation = ConnectionsHandler.CheckParent(out possibleComplexRel);
            if (!relation.HasValue) relation = ConnectionsHandler.CheckChild(out possibleComplexRel);
            if (!relation.HasValue) relation = ConnectionsHandler.CheckSibling(out possibleComplexRel);
            if (!relation.HasValue) relation = ConnectionsHandler.CheckSpouse(out possibleComplexRel);
            if (!relation.HasValue) relation = ConnectionsHandler.CheckParentSibling(out possibleComplexRel);
            if (!relation.HasValue) relation = eRel.FarRel;
            return relation.Value;
        }

        private static bool ConnExists(PersonDTO person, PersonDTO related, eRel relation, ref List<ConnectionDTO> newConnections)
        {
            var existsInNew = newConnections.Exists(c => c.TargetPerson.Id == person.Id && c.RelatedPerson.Id == related.Id && c.Relationship.Type == relation);
            var existsInAll = _allConnections.Exists(c => c.TargetPerson.Id == person.Id && c.RelatedPerson.Id == related.Id && c.Relationship.Type == relation);
            return existsInNew || existsInAll;
        }
        #endregion

        #region Has methods
        private static bool HasSpouse(ConnectionDTO connection)
        {
            return connection.Relationship.Type == eRel.Wife || _relatedConnection.Relationship.Type == eRel.Husband;
        }
        private static bool HasChild(ConnectionDTO connection)
        {
            return connection.Relationship.Type == eRel.Daughter || connection.Relationship.Type == eRel.Son;
        }
        private static bool HasParent(ConnectionDTO connection)
        {
            return connection.Relationship.Type == eRel.Mother || connection.Relationship.Type == eRel.Father;
        }
        private static bool HasSibling(ConnectionDTO connection)
        {
            return connection.Relationship.Type == eRel.Sister || connection.Relationship.Type == eRel.Brother;
        }
        private static bool HasSpouseInLaw(ConnectionDTO connection)
        {
            return connection.Relationship.Type == eRel.SisterInLaw || connection.Relationship.Type == eRel.BrotherInLaw;
        }
        private static bool HasSiblingChild(ConnectionDTO connection)
        {
            return connection.Relationship.Type == eRel.Niece || connection.Relationship.Type == eRel.Nephew;
        }
        private static bool HasSiblingInLaw(ConnectionDTO connection)
        {
            return connection.Relationship.Type == eRel.SisterInLaw || connection.Relationship.Type == eRel.BrotherInLaw;
        }
        private static bool HasParentSibling(ConnectionDTO connection)
        {
            return connection.Relationship.Type == eRel.Aunt || connection.Relationship.Type == eRel.Uncle;
        }
        #endregion

        #region Relation Checkers
        private static eRel? CheckParent(out eRel? possibleComplexRel) //, ref List<(ConnectionDTO Target, ConnectionDTO Related)> unknownsConnections_debug)
        {
            eRel? relation = null;
            possibleComplexRel = null;

            //person's Parent(Mother, Father)
            if (HasParent(_personConnection))
            {
                //HasParent
                if (HasParent(_relatedConnection))
                {
                    //GrandParent(GrandMother, GarndFather)
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.GrandMother : eRel.GrandFather;
                }
                //HasChild
                else if (HasChild(_relatedConnection))
                {
                    //Sibling(Sister, Brother)
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Sister : eRel.Brother;
                }
                //HasSibling
                else if (HasSibling(_relatedConnection))
                {
                    //ParentSibling(Aunt, Uncle)
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Aunt : eRel.Uncle;
                }
                //HasSpouse
                else if (HasSpouse(_relatedConnection))
                {
                    //Parent
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Mother : eRel.Father;
                    //StepParent
                    possibleComplexRel = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.StepMother : eRel.StepFather;
                }
                //HasSiblingInLaw
                else if (HasSiblingInLaw(_relatedConnection))
                {
                    //Sibling
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Aunt : eRel.Uncle;
                }
                else
                {
                    //unknownsConnections_debug.Add((_personConnection, _relatedConnection));
                    relation = eRel.FarRel;
                }
            }

            return relation;
        }
        private static eRel? CheckChild(out eRel? possibleComplexRel) //, ref List<(ConnectionDTO Target, ConnectionDTO Related)> unknownsConnections_debug)
        {
            eRel? relation = null;
            possibleComplexRel = null;

            //person's Child(Daughter, Son)
            if (HasChild(_personConnection))
            {
                //HasParent
                if (HasParent(_relatedConnection))
                {
                    //Spouse(Wife, Husband)
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Wife : eRel.Husband;
                    //ExPartner
                    possibleComplexRel = eRel.ExPartner;
                }
                //HasChild
                else if (HasChild(_relatedConnection))
                {
                    //GrandChild
                    relation = eRel.GrandChild;
                }
                //HasSibling
                else if (HasSibling(_relatedConnection))
                {
                    //Child
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Daughter : eRel.Son;
                    //StepChild
                    possibleComplexRel = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.StepDaughter : eRel.StepSon;
                }
                //HasSpouse
                else if (HasSpouse(_relatedConnection))
                {
                    //ChildInLaw
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.DaughterInLaw : eRel.SonInLaw;
                }
                else
                {
                    //unknownsConnections_debug.Add((_personConnection, _relatedConnection));
                    relation = eRel.FarRel;
                }
            }

            return relation;
        }
        private static eRel? CheckSibling(out eRel? possibleComplexRel) //, ref List<(ConnectionDTO Target, ConnectionDTO Related)> unknownsConnections_debug)
        {
            eRel? relation = null;
            possibleComplexRel = null;

            //person's Sibling
            if (HasSibling(_personConnection))
            {
                //HasParent
                if (HasParent(_relatedConnection))
                {
                    //Parent
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Mother : eRel.Father;
                    //StepParent
                    possibleComplexRel = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.StepMother : eRel.StepFather;
                }
                //HasChild
                else if (HasChild(_relatedConnection))
                {
                    //SiblingChild(Niece, Nephew)
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Niece : eRel.Nephew;
                }
                //HasSibling
                else if (HasSibling(_relatedConnection))
                {
                    //Sibling
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Sister : eRel.Brother;
                    //StepSibling
                    possibleComplexRel = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.SisterInLaw : eRel.BrotherInLaw;
                }
                //HasSpouse
                else if (HasSpouse(_relatedConnection))
                {
                    //SiblingInLaw
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.SisterInLaw : eRel.BrotherInLaw;
                }
                //HasParentSibling
                else if (HasParentSibling(_relatedConnection))
                {
                    //HasParentSibling
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Aunt : eRel.Uncle;
                }
                else
                {
                    //unknownsConnections_debug.Add((_personConnection, _relatedConnection));
                    relation = eRel.FarRel;
                }
            }

            return relation;
        }
        private static eRel? CheckSpouse(out eRel? possibleComplexRel) //, ref List<(ConnectionDTO Target, ConnectionDTO Related)> unknownsConnections_debug)
        {
            eRel? relation = null;
            possibleComplexRel = null;

            //person's Spouse
            if (HasSpouse(_personConnection))
            {
                //HasParent
                if (HasParent(_relatedConnection))
                {
                    //ParentInLaw
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.MotherInLaw : eRel.FatherInLaw;
                }
                //HasChild
                else if (HasChild(_relatedConnection))
                {
                    //Child
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Daughter : eRel.Son;
                    //StepChild
                    possibleComplexRel = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.StepDaughter : eRel.StepSon;
                }
                //HasSibling
                else if (HasSibling(_relatedConnection))
                {
                    //SiblingInLaw
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.SisterInLaw : eRel.BrotherInLaw;
                }
                //HasSpouseInLaw //HasSiblingChild
                else if (HasSpouseInLaw(_relatedConnection) || HasSiblingChild(_relatedConnection))
                {
                    //SpouseInLaw //SpouseSiblingChild
                    relation = eRel.FarRel;
                }
                else
                {
                    //unknownsConnections_debug.Add((_personConnection, _relatedConnection));
                    relation = eRel.FarRel;
                }
            }

            return relation;
        }
        private static eRel? CheckParentSibling(out eRel? possibleComplexRel) //, ref List<(ConnectionDTO Target, ConnectionDTO Related)> unknownsConnections_debug)
        {
            eRel? relation = null;
            possibleComplexRel = null;

            //person's ParentSibling
            if (HasParentSibling(_personConnection))
            {
                //HasParent
                if (HasParent(_relatedConnection))
                {
                    //GrandParent
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.GrandMother : eRel.GrandFather;
                }
                //HasChild
                else if (HasChild(_relatedConnection))
                {
                    //Cousin
                    relation = eRel.Cousin;
                }
                //HasSibling
                else if (HasSibling(_relatedConnection))
                {
                    //Parent
                    relation = _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Mother : eRel.Father;
                }
                else
                {
                    //unknownsConnections_debug.Add((_personConnection, _relatedConnection));
                    relation = eRel.FarRel;
                }
            }

            return relation;
        }
        #endregion
    }
}
