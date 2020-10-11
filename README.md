# An Evaluation of Unity ML-Agents Toolkit for Learning Boss Strategies.

A repository for the Unity project implemented for the thesis for my MSc. in Computer Science. 

For more information about the thesis, please refer to the following public repository link: http://hdl.handle.net/1946/37111

Feel free to use the same link to cite the project if it is useful to you.

## Abstract
Accompanying the growing pace of AI research for video games is the development of new benchmark environments. One of the most recently introduced environments is Unity’s Machine Learning Toolkit (ML-Agents Toolkit). With this toolkit, Unity allows its users (researchers or game developers) to incorporate state-of-the-art Reinforcement Learning or Imitation Learning algorithms or one’s own Machine Learning algorithms to train a learning agent. On this project I used one of the Reinforce Learning algorithms (Proximal Policy Optimization ; PPO) alone and in combination with two Imitation Learning algorithms (Generative Adversarial Imitation Learning ; GAIL and Behavioral Cloning ; BC) provided with Unity’s Machine Learning Toolkit. These were used to teach a learning agent how to optimize its policy in order to maximize its reward by learning how to better choose from a set of attacks to win in a fight against a simpler non-learning agent. The project has two focuses: a) To compare the learning provided by the different algorithms included in Unity’s toolkit, and additionally compare the use of the Imitation Learning algorithms as complements of the Reinforce Learning algorithm, and; b) Test the usability of the ML-Agents Toolkit by creating a learning environment to train an agent and compare my experience implementing and training such an agent with the information provided by Unity’s documentation. To achieve this, I conducted three case studies, one providing a demonstration file containing an optimal policy, one with a sub-optimal policy, and a third one with a mix of both. For all study cases, the learning was done considering four combinations of learning algorithms: a) PPO alone; b) PPO in combination with GAIL; c) PPO in combination with BC, and; d) all learning algorithms. The overall result of the three case studies showed a successful learning by the agent, regardless of the learning algorithms considered. From the Imitation Learning algorithms, GAIL showed difficulties to learn policies which involved several complex actions, whereas BC greatly increased the rate of learning. The results of the project show the advantages and limitations of the use of Imitation Learning algorithms for learning behaviours, the importance of the demonstration provided for the Imitation Learning algorithms, and further discusses the usefulness of entropy as a complementary variable to consider when assessing the success rate of the learning process.
