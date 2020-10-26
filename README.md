# Neural-Network
Artificial Multilayer Neural Network from scratches.
The network consists of an input layer, one hidden layer and an output layer.
Sigmoid function is used for activation.

## Snake Environment
Classic snake game with simple rules: if snake hits a wall or it's tail - it dies, if it finds food, it grows, and if it's unable to find food for a while, it will also die.
Snake can see if one field ahead, right and left is safe to move on (field value is 1 or 0). Snake also knows the direction of food - positive or negative value of the normalized angle between the snake's head and the food. Finally, it's aware of it's body length and has 'sense of apetite' - value that increases with time, and drops after eating. 

## Neuro-evolution
Neuro-evolution is an unsupervised learning method for Neural Networks. The general purpose of [genetic algorithms](https://github.com/takado8/Tetris#genetic-algorithm) is to optimize a function - neural networks are basically a big functions, with thousands or millions of weights. These parameters can be searched for with standard genetic algorithm. 
Set of randomly initialized networks (population) is tested in the snake world - without any prior knowledge about the environment. Most of random actions is lethal, at first most snakes hit the wall or spin in place and starve to death. After a while though, some will eventually randomly collect food, gain points and be on advantage in next replication. Genetic set that forces subject to collect food and avoid walls will be most fitted for this environment and will have better chance to replicate and replace less efficient ones.  

![img](https://github.com/takado8/Neural-Network/blob/master/Snake/Snake2/imgs/early_generation.gif)  ![img](https://github.com/takado8/Neural-Network/blob/master/Snake/Snake2/imgs/spinners.gif)  ![img](https://github.com/takado8/Neural-Network/blob/master/Snake/Snake2/imgs/spinners3.gif)

After several generations,  population starts to improve. Some snakes developed even real snake-like movements to compress their body.

![img](https://github.com/takado8/Neural-Network/blob/master/Snake/Snake2/imgs/better-generation-online-video-c.gif)  ![img](https://github.com/takado8/Neural-Network/blob/master/Snake/Snake2/imgs/better-generation2-online-video.gif) ![img](https://github.com/takado8/Neural-Network/blob/master/Snake/Snake2/imgs/snake_moves.gif)

## Digit recognition

Recognizes digit drawn by a user and appends it to training data.
Network trained on very small data set of 200 images generated manually, performs with 95% accuracy.
In such a small dataset, the result may be sensitive to differences between the user's handwriting and the creator's of the dataset.

![img](https://github.com/takado8/Neural-Network/blob/master/DigitReco/digit_reco.gif)
