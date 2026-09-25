using System;

namespace nunittest1;
public class ClassUnderTest{

    private IAffectingClass _iAff;  
    
    public int PublicMethod(int arg){

        return PrivateMethod(arg);
    }

    
    protected int ProtectedMethod(int arg){

        if(arg!=0)
        return _iAff.Val;
        else throw new ArgumentException("arg is equal to zero", nameof(arg));
    }

    private int PrivateMethod(int arg){

       	return _iAff.Val > arg ?  _iAff.Val: arg;
    }

    public int CallStatic(){

        return IAffectingClass.StaticDependency();
    }

   public  ClassUnderTest(IAffectingClass pAff){

        _iAff=pAff;
    }
}


public class ClassUnderTest2{

      

    AffectingClass aff_instance;

    public  ClassUnderTest2(AffectingClass pAff_instance){

        aff_instance=pAff_instance;
    }

    public int CallAffectingMethod(){

    	return aff_instance.Method();
   }

}