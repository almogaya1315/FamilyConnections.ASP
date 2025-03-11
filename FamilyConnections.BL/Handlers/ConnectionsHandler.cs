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
            if (relation == eRel.FarRel || relation == eRel.Undecided) return;

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
        public static eRel FindRelation(out eRel? possibleComplexRel, ref List<(ConnectionDTO Target, ConnectionDTO Related, string undecidedStr, List<eRel> options)> undecidedConnections)
        {
            var relation = CheckParent(out possibleComplexRel, ref undecidedConnections);
            if (!relation.HasValue) relation = CheckChild(out possibleComplexRel, ref undecidedConnections);
            if (!relation.HasValue) relation = CheckSibling(out possibleComplexRel, ref undecidedConnections);
            if (!relation.HasValue) relation = CheckSpouse(out possibleComplexRel, ref undecidedConnections);
            if (!relation.HasValue) relation = CheckParentSibling(out possibleComplexRel, ref undecidedConnections);
            if (!relation.HasValue) relation = CheckSiblingInLaw(out possibleComplexRel, ref undecidedConnections);
            if (!relation.HasValue) relation = FarRelation(ref undecidedConnections);
            return relation.Value;
        }

        private static bool ConnExists(PersonDTO person, PersonDTO related, eRel relation, ref List<ConnectionDTO> newConnections)
        {
            var existsInNew = newConnections.Exists(c => c.TargetPerson.Id == person.Id && c.RelatedPerson.Id == related.Id && c.Relationship.Type == relation);
            var existsInAll = _allConnections.Exists(c => c.TargetPerson.Id == person.Id && c.RelatedPerson.Id == related.Id && c.Relationship.Type == relation);
            return existsInNew || existsInAll;
        }
        private static string FarRelStr(eRel relation)
        {
            return $"{_personConnection.TargetPerson.FullName}'s {_personConnection.Relationship.Type}, {_personConnection.RelatedPerson.FullName}, Has a {_relatedConnection.Relationship.Type}, " +
                $"So {_relatedConnection.RelatedPerson.FullName} is {_personConnection.TargetPerson.FullName}'s {relation}";
        }
        public static eRel FarRelation(ref List<(ConnectionDTO Target, ConnectionDTO Related, string farRelStr, List<eRel> options)> undecidedConnections)
        {
            var relation = eRel.FarRel;
            undecidedConnections.Add((_personConnection, _relatedConnection, FarRelStr(relation), new List<eRel> { relation }));
            return relation;
        }
        private static eRel UndecidedRelation((eRel Female, eRel Male) rel1, (eRel Female, eRel Male) rel2, ref List<(ConnectionDTO Target, ConnectionDTO Related, string undecidedStr, List<eRel> options)> undecidedConnections)
        {
            var relation1 = _relatedConnection.RelatedPerson.Gender == eGender.Female ? rel1.Female : rel1.Male;
            var farRelStr1 = FarRelStr(relation1);
            var relation2 = _relatedConnection.RelatedPerson.Gender == eGender.Female ? rel2.Female : rel2.Male;
            var farRelStr2 = FarRelStr(relation2);

            var to = $"{_relatedConnection.RelatedPerson.FullName} to {_personConnection.TargetPerson.FullName}";
            undecidedConnections.Add((_personConnection, _relatedConnection, $"{to}, {relation1}{Environment.NewLine}{relation2}", new List<eRel> { relation1, relation2 }));
            return eRel.Undecided;
        }
        #endregion

        #region Has methods
        private static bool HasSpouse(ConnectionDTO connection)
        {
            return connection.Relationship.Type == eRel.Wife || connection.Relationship.Type == eRel.Husband;
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

        #region Is methods
        private static eRel IsGrandParent()
        {
            return _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.GrandMother : eRel.GrandFather;
        }
        private static eRel IsSibling()
        {
            return _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Sister : eRel.Brother;
        }
        private static eRel IsParentSibling()
        {
            return _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Aunt : eRel.Uncle;
        }
        private static eRel IsParent()
        {
            return _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Mother : eRel.Father;
        }
        private static eRel IsStepParent()
        {
            return _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.StepMother : eRel.StepFather;
        }
        private static eRel IsSiblingChild()
        {
            return _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Niece : eRel.Nephew;
        }
        private static eRel IsSiblingInLaw()
        {
            return _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.SisterInLaw : eRel.BrotherInLaw;
        }
        private static eRel IsChild()
        {
            return _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Daughter : eRel.Son;
        }
        private static eRel IsStepChild()
        {
            return _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.StepDaughter : eRel.StepSon;
        }
        private static eRel IsParentInLaw()
        {
            return _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.MotherInLaw : eRel.FatherInLaw;
        }
        private static eRel IsStepSibling()
        {
            return _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.StepSister : eRel.StepBrother;
        }
        private static eRel IsChildInLaw()
        {
            return _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.DaughterInLaw : eRel.SonInLaw;
        }
        private static eRel IsSpouse()
        {
            return _relatedConnection.RelatedPerson.Gender == eGender.Female ? eRel.Wife : eRel.Husband;
        }
        #endregion

        #region Relation Checkers
        private static eRel? CheckParent(out eRel? possibleComplexRel, ref List<(ConnectionDTO Target, ConnectionDTO Related, string undecidedStr, List<eRel> options)> undecidedConnections)
        {
            eRel? relation = null;
            possibleComplexRel = null;

            //relation = HasParent(_personConnection) ?
            //    (
            //        HasParent(_relatedConnection) ? IsGrandParent() : 
            //        (HasChild(_relatedConnection) ? IsSibling() : 
            //        (HasSibling(_relatedConnection) || HasSiblingInLaw(_relatedConnection) ? IsParentSibling() : 
            //        (HasSpouse(_relatedConnection) ? IsParent() : 
            //        FarRelation(ref undecidedConnections))))
            //    ) : relation;

            //person's Parent
            if (HasParent(_personConnection))
            {
                //HasParent
                if (HasParent(_relatedConnection))
                {
                    //IsGrandParent
                    relation = IsGrandParent();
                }
                //HasChild
                else if (HasChild(_relatedConnection))
                {
                    //IsSibling
                    relation = IsSibling();
                }
                //HasSibling //HasSiblingInLaw
                else if (HasSibling(_relatedConnection) || HasSiblingInLaw(_relatedConnection))
                {
                    //IsParentSibling
                    relation = IsParentSibling();
                }
                //HasSpouse
                else if (HasSpouse(_relatedConnection))
                {
                    //IsParent
                    relation = IsParent();
                    //IsStepParent
                    possibleComplexRel = IsStepParent();
                }
                else
                {
                    //FarRelation
                    relation = FarRelation(ref undecidedConnections);
                }
            }

            return relation;
        }
        private static eRel? CheckChild(out eRel? possibleComplexRel, ref List<(ConnectionDTO Target, ConnectionDTO Related, string undecidedStr, List<eRel> options)> undecidedConnections)
        {
            eRel? relation = null;
            possibleComplexRel = null;

            //person's Child
            if (HasChild(_personConnection))
            {
                //HasParent
                if (HasParent(_relatedConnection))
                {
                    //IsSpouse
                    relation = IsSpouse(); 
                    //IsExPartner
                    possibleComplexRel = eRel.ExPartner;
                }
                //HasChild
                else if (HasChild(_relatedConnection))
                {
                    //IsGrandChild
                    relation = eRel.GrandChild;
                }
                //HasSibling
                else if (HasSibling(_relatedConnection))
                {
                    //IsChild
                    relation = IsChild();
                    //IsStepChild
                    possibleComplexRel = IsStepChild();
                }
                //HasSpouse
                else if (HasSpouse(_relatedConnection))
                {
                    //IsChildInLaw
                    relation = IsChildInLaw(); 
                }
                else
                {
                    //FarRelation
                    relation = FarRelation(ref undecidedConnections);
                }
            }

            return relation;
        }

        private static eRel? CheckSibling(out eRel? possibleComplexRel, ref List<(ConnectionDTO Target, ConnectionDTO Related, string undecidedStr, List<eRel> options)> undecidedConnections)
        {
            eRel? relation = null;
            possibleComplexRel = null;

            //person's Sibling
            if (HasSibling(_personConnection))
            {
                //HasParent
                if (HasParent(_relatedConnection))
                {
                    //IsParent
                    relation = IsParent();
                    //IsStepParent
                    possibleComplexRel = IsStepParent(); 
                }
                //HasChild
                else if (HasChild(_relatedConnection))
                {
                    //IsSiblingChild
                    relation = IsSiblingChild(); 
                }
                //HasSibling
                else if (HasSibling(_relatedConnection))
                {
                    //IsSibling
                    relation = IsSibling(); 
                    //IsStepSibling
                    possibleComplexRel = IsStepSibling();  
                }
                //HasSpouse
                else if (HasSpouse(_relatedConnection))
                {
                    //IsSiblingInLaw
                    relation = IsSiblingInLaw(); 
                }
                //HasParentSibling
                else if (HasParentSibling(_relatedConnection))
                {
                    //IsParentSibling
                    relation = IsParentSibling();
                }
                //HasSiblingInLaw
                else if (HasSiblingInLaw(_relatedConnection))
                {
                    //IsSiblingInLaw
                    relation = IsSiblingInLaw(); 
                }
                //HasSiblingChild
                else if (HasSiblingChild(_relatedConnection))
                {
                    //IsSiblingChild
                    relation = IsSiblingChild(); 
                }
                else
                {
                    //FarRelation
                    relation = FarRelation(ref undecidedConnections);
                }
            }

            return relation;
        }

        private static eRel? CheckSpouse(out eRel? possibleComplexRel, ref List<(ConnectionDTO Target, ConnectionDTO Related, string undecidedStr, List<eRel> options)> undecidedConnections)
        {
            eRel? relation = null;
            possibleComplexRel = null;

            //person's Spouse
            if (HasSpouse(_personConnection))
            {
                //HasParent
                if (HasParent(_relatedConnection))
                {
                    //IsParentInLaw
                    relation = IsParentInLaw(); 
                }
                //HasChild
                else if (HasChild(_relatedConnection))
                {
                    //IsChild
                    relation = IsChild();
                    //IsStepChild
                    possibleComplexRel = IsStepChild(); 
                }
                //HasSibling
                else if (HasSibling(_relatedConnection))
                {
                    //IsSiblingInLaw
                    relation = IsSiblingInLaw();
                }
                else
                {
                    //FarRelation
                    relation = FarRelation(ref undecidedConnections);
                }
            }

            return relation;
        }

        private static eRel? CheckParentSibling(out eRel? possibleComplexRel, ref List<(ConnectionDTO Target, ConnectionDTO Related, string undecidedStr, List<eRel> options)> undecidedConnections)
        {
            eRel? relation = null;
            possibleComplexRel = null;

            //person's ParentSibling
            if (HasParentSibling(_personConnection))
            {
                //HasParent
                if (HasParent(_relatedConnection))
                {
                    //IsGrandParent
                    relation = IsGrandParent();
                }
                //HasChild
                else if (HasChild(_relatedConnection))
                {
                    //IsCousin
                    relation = eRel.Cousin;
                }
                //HasSibling
                else if (HasSibling(_relatedConnection))
                {
                    //IsParent
                    relation = IsParent();
                }
                //HasSiblingChild
                else if (HasSiblingChild(_relatedConnection))
                {
                    //IsSiblingChild
                    relation = IsSiblingChild();
                }
                //HasSpouse
                else if (HasSpouse(_relatedConnection))
                {
                    //IsParentSibling
                    relation = IsParentSibling();
                }
                //HasSiblingInLaw
                else if (HasSiblingInLaw(_relatedConnection))
                {
                    //ParentSibling OR Parent -> Undecided
                    relation = UndecidedRelation((eRel.Aunt, eRel.Uncle), (eRel.Mother, eRel.Father), ref undecidedConnections);
                }
                else
                {
                    //FarRelation
                    relation = FarRelation(ref undecidedConnections);
                }
            }

            return relation;
        }

        private static eRel? CheckSiblingInLaw(out eRel? possibleComplexRel, ref List<(ConnectionDTO Target, ConnectionDTO Related, string undecidedStr, List<eRel> options)> undecidedConnections)
        {
            eRel? relation = null;
            possibleComplexRel = null;

            //person's SiblingInLaw
            if (HasSiblingInLaw(_personConnection))
            {
                //HasParentSibling
                if (HasParentSibling(_relatedConnection))
                {
                    //IsParentSibling
                    relation = IsParentSibling();
                }
            }
            else
            {
                //FarRelation
                relation = FarRelation(ref undecidedConnections);
            }

            return relation;
        }
        #endregion
    }
}
