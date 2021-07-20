# Tiny Room Cleanup

## Scripts, soundtracks, and models are made by me and are free for use to everyone.

## How to use cleaning mechanic:

CleaningAction script does the thing. It renders what "Renderer Camera" sees onto a "RenderTexture". 
I preferred to use a transparent textured object to render its alpha onto the RenderTexture to achieve the erasing effect.

Every time the player holds the left mouse button, the script check for all pixels from the texture if they are less than the specified alpha.

Be aware: The render camera should only be able to see the texture we want to render and the transparent object.

#### Sound effects: [Mixkit](https://mixkit.co/) 
#### Contact: marswindgames@gmail.com
#### Website: [MarsWindGames](https://marswindgames.wordpress.com/)
Soundcloud: [Semih M. Soysal](https://soundcloud.com/semihmertsoysal)
