namespace nunittest1;

public interface IAffectingClass{

	public int Val{get; set;}

	public static int StaticProperty{get; private set;}

	public int Method();

	public static int StaticDependency()=>2;

	private int PrivateMethod()=>0;

}

public class AffectingClass:IAffectingClass{

	public int Val {get; set;}=0;

	public  virtual int Method()=>1;



}