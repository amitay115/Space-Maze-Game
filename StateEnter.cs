using UnityEngine;

public class StateEnter : StateMachineBehaviour
{
	bool IsEnter = false;
	public int[] length;
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		IsEnter = false;
		SendMsg(animator, stateInfo);
	}
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
	public void SendMsg(Animator animator, AnimatorStateInfo stateInfo)
	{
		if (IsEnter)
			return;
		if ((int)animator.GetFloat("item") > 1)
			animator.SetFloat("random", 0f);
		//else
			//animator.SetFloat("random", Random.Range(0, length[(int)animator.GetFloat("item")] + 1));
		IsEnter = true;
	}
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		IsEnter = false;
	}
}
