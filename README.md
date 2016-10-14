Description of World:
  My scene takes place on the edge of one of the rocky rings of a planet in space. The [temporary] population of this area of space includes a group of Daleks (evil mutant-cyborg warriors), their leader, and the Doctor in his TARDIS (Time-traveling space ship).  The Daleks (robot looking objects) follow their leader, who chases the TARDIS (blue police box). The Daleks chase the TARDIS because the only threat they see in the way of their universal genocide campaign is the Doctor.  The Doctor flies away from the Daleks because he doesn’t want to have to fight them right now.  This particular area of space is also home to a sort of space current (think Finding Nemo in space), which the TARDIS is caught in.

Steering Behaviors:
  Flock implementation: Daleks that use separation, alignment, and cohesion.
Steering Behaviors:  Seeking, arrival (used in leader following), obstacle avoidance, advanced behaviors.
Advanced Steering Behavior 1: Leader Following
  Daleks seek a point behind the leader, and arrive at that point if they get close enough.  Separation keeps the Daleks from running into each other.  The leader does not acknowledge the followers in code because the weights are adjusted so that such code would be unnecessary.
Advanced Steering Behavior 2: Path Following
  The TARDIS utilizes complex path following.  If the projected future position of the TARDIS is outside the radius of the path, then it seeks the projection of that position onto the nearest path segment.  It checks that projection against projections onto other segments to make sure it’s the closest.  I tried to implement an optimization where it only checks against segments 2 ahead or behind the current one, but I couldn’t figure out why it was freezing Unity at the 5th waypoint every time, so I commented it out.  Sometimes there are too many obstacles in the path of the TARDIS, or the obstacles are in poor positions, and the TARDIS gets hung up for a few seconds navigating around them.

Characters’ response to other characters:
  The TARDIS is weighted to appear to be fleeing from the Daleks.  The Daleks follow the lead Dalek, who seeks the TARDIS.  The Daleks avoid colliding with each other while trying to move in the same direction.  There seems to always be at least one follower Dalek ahead of all the others, his name is Jeff.

Characters’ response to the environment:
  All characters avoid asteroids in their way. The TARDIS has been sucked into a sort of space current and travels through this current.
Other notes:
  Press C to switch cameras between Centroid follow and Free Move. For the Free Move camera, WASD moves you relative to your view plane, i.e. to go down look down and hit W. 
Resources:
  Tardis Model and Textures- http://tf3dm.com/3d-model/tardis-41964.html
  Dalek Model and Textures- http://www.models-resource.com/pc_computer/doctorwho/model/1884/
  Asteroids- https://www.assetstore.unity3d.com/en/#!/content/38913
  Skybox- https://www.assetstore.unity3d.com/en/#!/content/3392
