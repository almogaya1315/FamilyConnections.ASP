﻿using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FamilyConnections.Core.DTO
{
    public class ConnectionDTO
    {
        public PersonDTO TargetPerson { get; set; }
        public PersonDTO RelatedPerson { get; set; }
        public RelationshipInfo Relationship { get; set; }

        public ConnectionDTO(PersonDTO target, PersonDTO related, eRel? rel, eRel? posibleComplexRel = null)
        {
            TargetPerson = target;
            RelatedPerson = related;
            Relationship = new RelationshipInfo(rel, posibleComplexRel);
            Flat = new FlatConnection(TargetPerson.Id, RelatedPerson.Id, Relationship.Id);
        }
        public ConnectionDTO()
        {
            TargetPerson = new PersonDTO();
            RelatedPerson = new PersonDTO();
            Relationship = new RelationshipInfo();
        }
        private FlatConnection _flat;
        public FlatConnection Flat
        {
            get
            {
                return _flat;
            }
            set
            {
                _flat = value;
            }
        }

        public override string ToString()
        {
            return $"{RelatedPerson.FullName} is {TargetPerson.FullName}'s {Relationship.Type}";
        }
    }

    public class FlatConnection
    {
        public int TargetId { get; set; }
        public int RelatedId { get; set; }
        public int RelationshipId { get; set; }

        public FlatConnection(int targetId, int relatedId, int relId)
        {
            TargetId = targetId;
            RelatedId = relatedId;
            RelationshipId = relId;
        }

        public override string ToString()
        {
            return $"Target: {TargetId}, Related: {RelatedId}, RelId: {RelationshipId}";
        }
    }

    public class RelationshipInfo
    {
        public RelationshipInfo()
        {
            Id = -1;
        }

        public RelationshipInfo(eRel? type, eRel? possibleComplexRel)
        {
            Type = type;
            Id = Type.HasValue ? (int)Type : -1;
            PossibleComplexRel = possibleComplexRel;
        }

        public int Id { get; set; }
        public eRel? Type
        {
            get
            {
                return (eRel)Id;
            }
            set
            {
                Id = (int)value;
            }
        }
        public eRel? PossibleComplexRel { get; set; }

        public void SetError(eError error)
        {
            var field = error.GetType().GetField(error.ToString());
            var attribute = (DescriptionAttribute)field?.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
            Error = attribute.Description;
        }
        public string Error { get; set; }

        public static eRel Get(int relId)
        {
            return (eRel)relId;
        }

        public static eGender? Gender(string relIdStr)
        {
            var relId = int.Parse(relIdStr);
            var eRel = Enum.GetValues(typeof(eRel)).Cast<eRel>().First(e => (int)e == relId);
            return Gender(eRel);
        }

        public static eGender? Gender(eRel rel)
        {
            eGender? gender = null;

            switch (rel)
            {
                case eRel.Mother:
                case eRel.Sister:
                case eRel.Daughter:
                case eRel.Wife:
                case eRel.Aunt:
                case eRel.Niece:
                case eRel.MotherInLaw:
                case eRel.SisterInLaw:
                    gender = eGender.Female;
                    break;
                case eRel.Father:
                case eRel.Brother:
                case eRel.Son:
                case eRel.Husband:
                case eRel.Uncle:
                case eRel.Nephew:
                case eRel.FatherInLaw:
                case eRel.BrotherInLaw:
                    gender = eGender.Male;
                    break;
                case eRel.Cousin:
                    break;
            }

            return gender;
        }

        public int? FemaleRelId
        {
            get
            {
                return GetRelId(eGender.Female);
            }
            set
            {
                SetRelId(value, eGender.Female);
            }
        }
        public int? MaleRelId
        {
            get
            {
                return GetRelId(eGender.Male);
            }
            set
            {
                SetRelId(value, eGender.Male);
            }
        }

        private void SetRelId(int? value, eGender gender)
        {
            if (value.HasValue && value.Value > -1) Id = value.Value;
            if (gender == eGender.Male && FemaleRelId != null) FemaleRelId = null;
            if (gender == eGender.Female && MaleRelId != null) MaleRelId = null;
        }

        private int? GetRelId(eGender gender)
        {
            if (Id == -1) return null;
            int? output = Id;
            var gen = Gender(Type.Value);
            if (gen != gender) output = null;
            return output;
        }

        public static eRel Opposite(eRel relation, eGender gender)
        {
            eRel output = eRel.FarRel;

            switch (relation)
            {
                case eRel.Mother:
                case eRel.Father:
                    output = gender == eGender.Female ? eRel.Daughter : eRel.Son;
                    break;
                case eRel.Sister:
                case eRel.Brother:
                    output = gender == eGender.Female ? eRel.Sister : eRel.Brother;
                    break;
                case eRel.Daughter:
                case eRel.Son:
                    output = gender == eGender.Female ? eRel.Mother : eRel.Father;
                    break;
                case eRel.Wife:
                    output = eRel.Husband;
                    break;
                case eRel.Husband:
                    output = eRel.Wife;
                    break;
                case eRel.Aunt:
                case eRel.Uncle:
                    output = gender == eGender.Female ? eRel.Niece : eRel.Nephew;
                    break;
                case eRel.Cousin:
                    output = eRel.Cousin;
                    break;
                case eRel.Niece:
                case eRel.Nephew:
                    output = gender == eGender.Female ? eRel.Aunt : eRel.Uncle;
                    break;
                case eRel.GrandMother:
                case eRel.GrandFather:
                    output = eRel.GrandChild;
                    break;
                case eRel.GrandChild:
                    output = gender == eGender.Female ? eRel.GrandMother : eRel.GrandFather;
                    break;
                case eRel.MotherInLaw:
                case eRel.FatherInLaw:
                    output = gender == eGender.Female ? eRel.DaughterInLaw : eRel.SonInLaw;
                    break;
                case eRel.SisterInLaw:
                case eRel.BrotherInLaw:
                    output = gender == eGender.Female ? eRel.SisterInLaw : eRel.BrotherInLaw;
                    break;
                case eRel.DaughterInLaw:
                case eRel.SonInLaw:
                    output = gender == eGender.Female ? eRel.MotherInLaw : eRel.FatherInLaw;
                    break;
                case eRel.StepMother:
                case eRel.StepFather:
                    output = gender == eGender.Female ? eRel.StepDaughter : eRel.StepSon;
                    break;
                case eRel.StepSister:
                case eRel.StepBrother:
                    output = gender == eGender.Female ? eRel.StepSister : eRel.StepBrother;
                    break;
                case eRel.StepDaughter:
                case eRel.StepSon:
                    output = gender == eGender.Female ? eRel.StepMother : eRel.StepFather;
                    break;
                case eRel.ExPartner:
                    output = eRel.ExPartner;
                    break;
                case eRel.GreatGrandMother:
                case eRel.GreatGrandFather:
                    output = gender == eGender.Female ? eRel.GreatGrandDaughter : eRel.GreatGrandSon;
                    break;
                case eRel.GreatGrandDaughter:
                case eRel.GreatGrandSon:
                    output = gender == eGender.Female ? eRel.GreatGrandMother : eRel.GreatGrandFather;
                    break;
                case eRel.FarRel:
                    output = eRel.FarRel;
                    break;
            }

            return output;
        }
    }
}
