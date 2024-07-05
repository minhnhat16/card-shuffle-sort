Hello, Thanks for bying my asset!

This asset contains a wonderful custom layout group, which is much better than a default one because my own is working in every frame
but default one is working only when canvas is updating. This method of updating layout when canvas is updating makes several bugs
like when you spawn several objects while window with a layout is disabled and then you enable it - istead of ordered layout you 
will see only chaos! And this is only one of many issues of the default layout.


How to use:

In folder Custom Layout Group -> Scripts you would find AutoSizeLayout script which do the trick and make an ordered layout.
You only need to add this script on parent of children and enable IsLoopUpdate parameter to test layout and set the type of the layout.
If you need best performance - you don't need to enable updating a layout in Update. Instead you can launch UpdateLayout method once 
after you spawn objects or enable window.

Now, about settings:

-isLoopUpdate needs to update layout in Update
-dontTouchChildren is using if you dont need to control the children positions but only size of layout parent
-typeLayout is target type of layout (Vertical top, horizontal center,...)
-isResizeSelf needs to resize layout parent
-topPad, bottomPad, leftPad, rightPad - padding
-spacing is a space between elements
-repeatFrames is a number of frames you need to update layout after first launch of UpdateLayout()
-isHaveMinSizeX, isHaveMinSizeY is using if you need to set minimum size of the layout
-minSize - vector to controll minimum size values
-isHaveMaxSizeX, isHaveMaxSizeY is using if you need to set maximum size of the layout
-maxSize - vector to controll maximum size values
-minSizeTargetRect - special value if you need to set minimum size for layout as a size of target rect. Useful when you need layout to have 
					 minimum size as canvas heigth for instance

If you set a tag NotInLayout to a child object it will be excluded from calculation
But if you set the value isInverted to true - only objects with the tag NotInLayout will be included in calculation

Method UpdateLayout() has two parameters: 

-isRepeat is using if you need to update layout several times (repeatFrames) after first launch
-isRecursive is using if you need to launch a UpdateLayout() method in every child in the whole hierarchy 


Contacts:
Email: korobko416@gmail.com
Telegram: @Alexandr_Korobko