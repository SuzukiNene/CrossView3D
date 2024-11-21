# How to create Player_00 Prefab.

## Step.1 Create Empty GameObject.
- Name = Player_{xx}
- Attach FieldPlayerBehaviour script
## Step.2 Add 3D model as child
- Scale = {1.7, 1.7, 1.7}
- AnimatorController = FieldPlayer.controller
- ApplyRootMotion = OFF
- Attach PlayerAnimBehaviour script
## Step.3 Create Empty GameObject as child
- Name = ViewAngle
- Attach ViewAngleBehaviour script
  - Parameter: "Fan Shape Range" = 0.85
  - Parameter: "Material" = mat_FanShape
## Step.4 Add SpotLight as child of ViewAngle
- Height = 0.05
## Step.5 Drag Player_00 object into Prefabs folder  

EOF
