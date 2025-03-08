using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConnections.Core.Enums
{
    public enum eRel
    {
        Mother = 0,
        Father = 1,
        Sister = 2,
        Brother = 3,
        Daughter = 4,
        Son = 5,
        Wife = 6,
        Husband = 7,
        Aunt = 8,
        Uncle = 9,
        Cousin = 10,
        Niece = 11,
        Nephew = 12,
        GrandMother = 13,
        GrandFather = 14,
        GrandChild = 15,

        //eComplexRel
        MotherInLaw = 16,
        FatherInLaw = 17,
        SisterInLaw = 18,
        BrotherInLaw = 19,
        DaughterInLaw = 20,
        SonInLaw = 21,
        StepMother = 22,
        StepFather = 23,
        StepSister = 24,
        StepBrother = 25,
        StepDaughter = 26,
        StepSon = 27,
        ExPartner = 28,
        GreatGrandMother = 29,
        GreatGrandFather = 30,
        GreatGrandDaughter = 31,
        GreatGrandSon = 32,

        //eFarRel
        FarRel = 33
        //Wife_BrotherInLaw = 33,
        //Wife_SisterInLaw = 34,
        //Husband_BrotherInLaw = 35,
        //Husband_SisterInLaw = 36,
        //Wife_Niece = 37,
        //Wife_Nephew = 38,
        //Husband_Niece = 39,
        //Husband_Nephew = 40,
    }

    public enum eComplexRel
    {
        MotherInLaw = 16,
        FatherInLaw = 17,
        SisterInLaw = 18,
        BrotherInLaw = 19,
        DaughterInLaw = 20,
        SonInLaw = 21,
        StepMother = 22,
        StepFather = 23,
        StepSister = 24,
        StepBrother = 25,
        StepDaughter = 26,
        StepSon = 27,
        ExPartner = 28,
        GreatGrandMother = 29,
        GreatGrandFather = 30,
        GreatGrandDaughter = 31,
        GreatGrandSon = 32,
    }

    public enum eFarRel
    {
        FarRel = 33
        //Wife_BrotherInLaw = 33,
        //Wife_SisterInLaw = 34,
        //Husband_BrotherInLaw = 35,
        //Husband_SisterInLaw = 36,
        //Wife_Niece = 37,
        //Wife_Nephew = 38,
        //Husband_Niece = 39,
        //Husband_Nephew = 40,
    }
}
