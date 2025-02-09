using System;
using OdinPlus.Common;

namespace OdinPlus.Npcs.Humans
{
	public class HumanFighter : HumanNpc, Hoverable, Interactable, IOdinInteractable
	{
		protected override void Awake()
		{
			base.Awake();
			ChoiceList = new string[2] { "$op_talk", "$op_human_fight" };
			m_hum.m_onDamaged = (Action<float, Character>)Delegate.Combine(m_hum.m_onDamaged, (Action<float, Character>)Damage);
			m_hum.m_onDeath = (Action)Delegate.Combine(m_hum.m_onDeath, (Action)onDeath);
		}
		public override void Choice0()
		{
			Say("Want a Fight?", "emote_point");
		}

		public void Choice1()
		{
			Say("How dare you", "emote_point");
			ChangeFaction(Character.Faction.Boss);
		}
		private void Damage(float hit, Character character)
		{
			if (character == null)
			{
				return;
			}
			if (character.IsPlayer())
			{
				Choice1();
			}
		}
		public void onDeath()
		{

		}
	}
}
