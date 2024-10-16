# Exercise 2

### Crane Anatomy
![](https://github.com/TUAS-Duy/game-math-private/blob/f7d51d48f45970dc138f0d7e3e4cd627f86341e2/Images/crane-anatomy.png)

### Grading

Crane execute following sequence of actions when user click on `Concrete`:

1. `Crane` starts to rotate around world Y axis and stop when its arm facing `Concrete` in top down view
2. `Trolley` moves in position that makes `Cable` ready to pick up `Concrete`. `Trolley` movement is still constrained by near and far limit points
3. Adjust `Cable` length so that `Hook` can reach `Concrete` and pick it up
4. Wait 1 second and gradually lift up the `Concrete` until it reaches `Cable` minimum length
5. Detach `Concete` and move it to a random location that the next sequence can reach it

| Grade | Mechanics |
| :---: | ------------- |
| 1 | Perform step 1 with maximum 1 degree of angle error |
| 2 | Perform upto step 2 with maximum 0.1 unit distance of error between `Trolley` position and `Concrete`'s attachment position. That distance is calculated by projecting both positions onto world plane (e.g: the plane at position zero and world up vector as normal) and then find the magnitude of the vector formed by those 2 projected points |
| 3 | Perform upto step 3 |
| 4 | Perform upto step 4 |
| 5 | Perform upto step 5. The validity of the random position can be checked by projecting it to world plane and compare that with projected inner and outer circles formed by `Trolley`'s near and far limits. The y position must be in range [10, 20]


### Note:
* `Transform` parenting is not allowed
* `ParentConstraint` component is not allowed
* No physics simulation is required in this exercise
* Collsion checking between `Hook` and `Concrete`'s attachment point can be achieve by rigidbody collision, rigidbody trigger zone, distance checking or by any other creative methods
* `Cable` mechanics can be done with `LineRenderer` component or provided cable model
