namespace Sample.Fact.InSkill.Purchases.Data;

public record FactItem(string Type, string Fact);

public static class FactData
{
    public static readonly FactItem[] AllFacts =
    [
        new("free", "There are 365 days in a year, except leap years, which have 366 days."),
        new("free", "What goes up, must come down. Except when it doesn't."),
        new("free", "Two wrongs don't make a right, but three lefts do."),
        new("free", "There are 24 hours in a day."),
        new("science", "There is enough DNA in an average person's body to stretch from the sun to Pluto and back — 17 times."),
        new("science", "The average human body carries ten times more bacterial cells than human cells."),
        new("science", "It can take a photon 40,000 years to travel from the core of the sun to its surface, but only 8 minutes to travel the rest of the way to Earth."),
        new("science", "At over 2000 kilometers long, The Great Barrier Reef is the largest living structure on Earth."),
        new("science", "There are 8 times as many atoms in a teaspoonful of water as there are teaspoonfuls of water in the Atlantic ocean."),
        new("science", "The average person walks the equivalent of five times around the world in a lifetime."),
        new("science", "When Helium is cooled to absolute zero it flows against gravity and will start running up and over the lip of a glass container!"),
        new("science", "An individual blood cell takes about 60 seconds to make a complete circuit of the body."),
        new("science", "The longest cells in the human body are the motor neurons. They can be up to 4.5 feet long and run from the lower spinal cord to the big toe."),
        new("science", "The human eye blinks an average of 4,200,000 times a year."),
        new("history", "The Hundred Years War actually lasted 116 years from thirteen thirty seven to fourteen fifty three."),
        new("history", "There are ninety two known cases of nuclear bombs lost at sea."),
        new("history", "Despite popular belief, Napoleon Bonaparte stood 5 feet 6 inches tall. Average height for men at the time."),
        new("history", "Leonardo Da Vinci designed the first helicopter, tank, submarine, parachute and ammunition igniter... Five hundred years ago."),
        new("history", "The shortest war on record was fought between Zanzibar and England in eighteen ninety six. Zanzibar surrendered after 38 minutes."),
        new("history", "X-rays of the Mona Lisa show that there are 3 different versions under the present one."),
        new("history", "At Andrew Jackson's funeral in 1845, his pet parrot had to be removed because it was swearing too much."),
        new("history", "English was once a language for commoners, while the British elites spoke French."),
        new("history", "In ancient Egypt, servants were smeared with honey in order to attract flies away from the pharaoh."),
        new("history", "Ronald Reagan was a lifeguard during high school and saved 77 people's lives."),
        new("space", "A year on Mercury is just 88 days long."),
        new("space", "Despite being farther from the Sun, Venus experiences higher temperatures than Mercury."),
        new("space", "Venus rotates anti-clockwise, possibly because of a collision in the past with an asteroid."),
        new("space", "On Mars, the Sun appears about half the size as it does on Earth."),
        new("space", "Earth is the only planet not named after a god."),
        new("space", "Jupiter has the shortest day of all the planets."),
        new("space", "The Milky Way galaxy will collide with the Andromeda Galaxy in about 5 billion years."),
        new("space", "The Sun contains 99.86% of the mass in the Solar System."),
        new("space", "The Sun is an almost perfect sphere."),
        new("space", "A total solar eclipse can happen once every 1 to 2 years. This makes them a rare event."),
    ];
}