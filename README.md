# vr-app-handtracking

Video Link: [VR Application](https://www.youtube.com/watch?v=o5yFdqBpLuQ)

![vr app hand tracking](./docs/vr_app_hand_tracking.gif)

* Virtual Reality Application implemented as part of my Master's degree thesis (2016).
    * Using: Oculus Rift DK2, Leap Motion, Unity 3D, Nvidia 3D Vision glasses, hand tracking, computer-human interaction

    * [Link: Master Degree Thesis](riunet.upv.es/handle/10251/77848 "Comparison of 3D visualization: Oculus vs Nvidia Glasses") *"Comparison of 3D visualization: Oculus Dk2 vs Nvidia 3D Glasses"*

    * Hands are an indispensable way for humans to interact with the environment in their daily lives. To incorporate the possibility that they can interact with their hands within a virtual world, it raises the degree of immersion and provides a form of natural interaction in virtual reality applications. In this final master work, we undertake the development of a virtual reality application in which stereoscopic visualization and hand interaction are used. At same time, we compare two modes of stereoscopic visualization to determine if participants cope with any of them better. We will observe if the results indicate that regardless of gender, age, or profession, participants prefer the mode with a virtual helmet
    
* Requirements
    * Windows 7 / 10
    * Unity 5
    * Oculus DK2
    * Leap Motion 2.1
    * Plugins for Leap Motion and SQLite are located at [Link: Plugins](https://github.com/GiovannyJTT/vr-app-handtracking/tree/main/Assets/Plugins "Plugins")
    * This project assets (audio, fonts, images, materials, models, prefabs, recordings, scenes, and scripts) are located [Link: Assets/Assets_tfm](https://github.com/GiovannyJTT/vr-app-handtracking/tree/main/Assets/Assets_tfm "Assets/Assets_tfm")
    * Code of this project is located at [Link: Assets/Assets_tfm/Scripts](https://github.com/GiovannyJTT/vr-app-handtracking/tree/main/Assets/Assets_tfm/Scripts "Assets/Assets_tfm/Scripts")

## Index

* Introduction
* Used tools
* Application developed
* Validations
* Conclusions

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0002.jpg)

## Introduction
### Virtual Reality

* “It is the product of the simulation of a
physical world created from programs
computers, computer systems,
graphics engines, which gives the possibility
interaction with humans through
different sensors” (Sherman & Craig, 2003)
* Virtual Reality Device (helmet)
* Stereoscopy with Active Glasses

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0003.jpg)

### Hand Interaction

* Use the hands as a natural interface to interact with the virtual objects increases the degree of immersion
* Users try to touch virtual objects
* Keyboard or mouse: handling difficulty, reduces the level of immersion

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0004.jpg)

### State of the Art
* Interaction with hands and screen:
    * Many ways to complete the game (Christian et al. 2011)
    * Avoid arms stretched out for a long time (Schlattmann et al., 2011)
* Interaction with hands and virtual reality headset:
    * Grasping pieces requires a complex algorithm (Lee et al., 2015)
    * Higher level of immersion (Lee et al., 2015)

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0005.jpg)

### Project Goals

1. Study the potential of Leap Motion for hand tracking
2. Develop an app that combines visualization stereoscopic and hands-on interaction
3. Validate the application with participants

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0006.jpg)

## Tools

### Description

* The developed application contains 3 games using hands-on interaction and 3D visualization

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0007.jpg)


### Unity 3D

* Cross-platform, abundant information, free vs personal
* Canvas (buttons, sliders, etc.)
* Preview of scene seen from VR helmet
* Import 3D models

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0008.jpg)

### Leap Motion

* 3 infrared LEDs, 2 cameras
* Leap Motion Core Assets:
    * Graphic part and physical part
    * Parent-child hierarchy
    * Avoid collisions between bones
    * Frame: data structure containing information geometry on the hands

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0009.jpg)

### Oculus Rift

* OVRCameraRig
    * LeftEyeAnchor
    * RightEyeAnchor
    * Center Anchor
        * HandController

* HandController
    * System of coordinates adjusted for your position on the head

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0010.jpg)

### Nvidia 3D vision

* Supported graphics card DirectX 11
* Dual link DVI cable
* Main camera:
    * Static (no tracking positional)
    * Field of vision according to the distance of objects
    * Stereo separation
    * Convergence distance

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0011.jpg)

### Blender, Gimp, Reaper

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0012.jpg)

#### Blender

* Union, intersection, difference
* Chamfered on the edges
* System of FBX coordinates are adjust automatically in unity

#### Gimp

* Histograms by channels
* Smart scissors
* Brushes, buckets of painting
* Brightness and Contrast

#### Reaper

* cut fragments of Audio
* Record instructions
* Remove background noise
* Equalizer (boost the voice)
* Compressor (avoids saturation in the recordings)

## Application Developed

### Description

* Database Manager
* Application Manager complete
* Manager of the Graphic Interface of User
* Museum Game Manager
* Soccer Game Manager
* Tower Game Manager

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0013.jpg)

### Data Base Manager

* Dynamic lists to avoid temporary delays to update the db while play

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0014.jpg)

### App Manager

* Information about the session current game:
    * Current person
    * 3D type
    * Date
    * Etc.

* Features common to several modules:
    * Set up:
        * Cameras depending on the type of 3D
        * Speed of the hands in the demonstrations
        * Appearance of instructions in text

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0015.jpg)

### Graphical User Interface

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0016.jpg)

### Game Museum

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0017.jpg)

### Game VolleyBall

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0018.jpg)

### Game Tower

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0019.jpg)

## Validations
### Description

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0020.jpg)

### Time used

* There are statistically significant differences
    * Student's t: p-value < 0.05
    * C_Oculus: median of 80 (and range interquartile of 30)
    * C_Glasses: median of 115 (and range interquartile of 26)

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0021.jpg)

### Interaction, Ergonomics and 3D quality, 1st time

* Oculus gets a superior rating (close to 7 – “Strongly agree”)

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0022.jpg)

### Comparison - Interaction

* Group A values both devices similarly

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0023.jpg)

### Comparison – 3D Quality

* There are no statistically significant differences
* The order of use of the devices has not influenced the evaluation of the quality of the 3D

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0024.jpg)

### Comparison - Preferences

* The difference in comfort is not as great as in the other questions

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0025.jpg)

### Comparison – Preferences (gender, profession, age)

* Neither gender, nor profession, nor age, influence the final preference of the participants

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0026.jpg)

## Conclusions

* We have developed a VR application that allows hands-on interaction and includes 2 types of 3D
* We have carried out a study in which we have determined that one of the devices favors a better interaction
* Feedback to the user through instructions, effects sounds, color changes of objects, etc., has prevented this from being look lost

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0027.jpg)

## Future work

* Validate the application with a larger number of participants
* Possibility of moving the camera depending on the movement of the hands
* Possibility of incorporating machine learning to detect gestures complex like drawing letters in the air

![vr app hand tracking](./docs/tfm_slides/tfm_presentacion_pages-to-jpg-0028.jpg)
