HOW TO USE ---------------------------------------------------
Make a .txt file in the "Executable\Input" folder, and put a list of numbers in it. 
Make sure each number is on a unique line. 
If you want to leave a line blank just put a 0.

The .exe in the Executable folder will turn any files in the "Input" folder into a base64 conversion that scrapmechanic can understand. Copy the entire base64 string and put it into the ("data":"[Past here]") section of a blueprint json file containing a memory block.
WARNING-------------------------------------------------------
Scrap Mechanic it self saves the data to memory blocks in a compressed form, this program however does not compress it so the output can get very long. Because it does not compress the files, 99% of the time, the process will not be able to be reverted to read files (That is NOT a feature of this program, but just a tip for anyone looking to expand onto this).
