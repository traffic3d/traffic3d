using System;

public static class SumoTrafficLightCharacterState
{
    public static CharacterStateAttribute GetAttribute(this CharacterState value)
    {
        Type type = value.GetType();
        System.Reflection.FieldInfo fieldInfo = type.GetField(value.ToString());
        var atts = (CharacterStateAttribute[])fieldInfo.GetCustomAttributes(typeof(CharacterStateAttribute), false);
        return atts.Length > 0 ? atts[0] : null;
    }

    public static TrafficLight.LightColour GetLightColourFromCharacter(char character)
    {
        foreach (CharacterState charState in Enum.GetValues(typeof(CharacterState)))
        {
            CharacterStateAttribute attribute = charState.GetAttribute();
            if (attribute.character.Equals(character))
            {
                return attribute.lightColour;
            }
        }
        return TrafficLight.LightColour.RED;
    }

    public static char GetCharacterFromLightColour(TrafficLight.LightColour lightColour)
    {
        foreach (CharacterState charState in Enum.GetValues(typeof(CharacterState)))
        {
            CharacterStateAttribute attribute = charState.GetAttribute();
            if (attribute.lightColour == lightColour && attribute.isMainConversion)
            {
                return attribute.character;
            }
        }
        return CharacterState.R_LOWER.GetAttribute().character;
    }

    public enum CharacterState
    {
        [CharacterStateAttribute('R', TrafficLight.LightColour.RED, false)]
        R_UPPER,
        [CharacterStateAttribute('r', TrafficLight.LightColour.RED, true)]
        R_LOWER,
        [CharacterStateAttribute('Y', TrafficLight.LightColour.AMBER, false)]
        Y_UPPER,
        [CharacterStateAttribute('y', TrafficLight.LightColour.AMBER, true)]
        Y_LOWER,
        [CharacterStateAttribute('G', TrafficLight.LightColour.GREEN, false)]
        G_UPPER,
        [CharacterStateAttribute('g', TrafficLight.LightColour.GREEN, true)]
        G_LOWER
    }

    public class CharacterStateAttribute : Attribute
    {
        public char character;
        public TrafficLight.LightColour lightColour;
        public bool isMainConversion;

        public CharacterStateAttribute(char character, TrafficLight.LightColour lightColour, bool isMainConversion)
        {
            this.character = character;
            this.lightColour = lightColour;
            this.isMainConversion = isMainConversion;
        }
    }
}
