# Jumper Agent - Simon Baeck | Stef Santens

#### Panopto video link: https://ap.cloud.panopto.eu/Panopto/Pages/Viewer.aspx?id=a4b406d9-196f-4972-9e21-ae7c0075eeb0

### Tutorial
1. Creëer een nieuw Unity 3D project.
2. Voeg volgende objecten toe:
![Image1](/images/img1.png)
3. Navigeer naar Window > Package manager
4. Selecteer bij "Packages" unity registery en installeer de ML Agents package.
5. Voeg volgende componenten toe aan het Agent object:
![Image2](/images/img2.png)
6. Voeg volgende componenten toe aan het obstakel en collectable object:
![Image3](/images/img3.png)
8. Stel volgende tags in op de objecten:
    - Agent -> Agent
    - Obstakel -> Obstacle
    - Obstakel trigger -> ObstacleTrigger
    - Collectable -> Collectable
    - Collectable trigger -> CollectableTrigger
    - Road -> Road
    - Agent zone -> AgentZone
9. Gebruik [DIT](https://github.com/AP-IT-GH/jumper-assignment-simonbaeck-stefsantens/blob/main/Assets/Scripts/AgentJumper.cs) script en voeg het toe als component aan de agent.
10. Maak een folder "config" aan in de root directory van het project.
11. Gebruik volgend YAML config bestand:
```yaml 
behaviors:
  AgentJump:
    trainer_type: ppo
    hyperparameters:
      batch_size: 10
      buffer_size: 1000
      learning_rate: 3.0e-4
      beta: 5.0e-4
      epsilon: 0.2
      lambd: 0.99
      num_epoch: 3
      learning_rate_schedule: linear
      beta_schedule: constant
      epsilon_schedule: linear
    network_settings:
      normalize: false
      hidden_units: 128
      num_layers: 2
    reward_signals:
      extrinsic:
        gamma: 0.99
        strength: 1.0
    max_steps: 2000000
    time_horizon: 64
    summary_freq: 10000
```
11. Start een nieuwe command line op in de config directory van het project en gebruik volgend commando om de agent te beginnen trainen:
```cmd
mlagents-learn config.yaml --run-id=Run01
```
12. Gebruik volgend commando in de config directory van het project om de resultaten van de training te bekijken:
```cmd
tensorboard --logdir results
```
