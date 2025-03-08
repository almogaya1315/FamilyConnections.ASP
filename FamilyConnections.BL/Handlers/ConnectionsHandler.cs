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

        private static ConnectionDTO _personConnection;
        private static ConnectionDTO _relatedConnection;

        public static void SetDTOs(ConnectionDTO personConnection, ConnectionDTO relatedConnection)
        {
            _personConnection = personConnection;
            _relatedConnection = relatedConnection;
        }

        public static eRel? CheckParent(out eRel? possibleComplexRel, ref List<(ConnectionDTO Target, ConnectionDTO Related)> unknownsConnections_debug)
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
                else
                {
                    unknownsConnections_debug.Add((_personConnection, _relatedConnection));
                }
            }

            return relation;
        }

        public static eRel? CheckChild(out eRel? possibleComplexRel, ref List<(ConnectionDTO Target, ConnectionDTO Related)> unknownsConnections_debug)
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
                    unknownsConnections_debug.Add((_personConnection, _relatedConnection));
                }
            }

            return relation;
        }

        public static eRel? CheckSibling(out eRel? possibleComplexRel, ref List<(ConnectionDTO Target, ConnectionDTO Related)> unknownsConnections_debug)
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
                else
                {
                    unknownsConnections_debug.Add((_personConnection, _relatedConnection));
                }
            }

            return relation;
        }

        public static eRel? CheckSpouse(out eRel? possibleComplexRel, ref List<(ConnectionDTO Target, ConnectionDTO Related)> unknownsConnections_debug)
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
                //HasSpouseInLaw
                else if (HasSpouseInLaw(_relatedConnection))
                {
                    //SpouseInLaw
                    relation = eRel.FarRel;
                }
                //HasSiblingChild
                else if (HasSiblingChild(_relatedConnection))
                {
                    //SpouseSiblingChild
                    relation = eRel.FarRel;
                }
                else
                {
                    unknownsConnections_debug.Add((_personConnection, _relatedConnection));
                }
            }

            return relation;
        }
    }
}
