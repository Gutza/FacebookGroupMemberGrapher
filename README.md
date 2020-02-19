# What is this?

This is a really simple tool which allows you to generate graphs showing who invited whom on your Facebook groups.
You need administrative access to the group you want to graph.

# How does it do that?

This is just a parser for "dead" text, and it works offline.
There is no online tool that needs access to your group or account, this is 100% DIY on your computer.

# What do I need?

Everything you need is open source, or at least free – you don't need to buy any software license. You will need:
1. Administrative access to a Facebook group
1. A computer (everything is cross-platform, the OS is irrelevant)
1. The [.NET Core SDK](https://dotnet.microsoft.com/download) – make sure you install the SDK, so you can build the sources!
1. Some software to [render DOT files](https://www.google.com/search?q=render+dot+files); I like [Gephi](https://gephi.org/), but there are several options
1. The [sources](https://github.com/Gutza/FacebookGroupMemberGrapher/archive/master.zip) for this project

# What do I get?

You get a DOT file, which you can then render using any number of graph generation tools, such as graphviz. I used Gephi to render this for a group I administer (and yes, you can render the names, too; I chose not to, for privacy reasons):

![Sample output using Gephi](https://user-images.githubusercontent.com/574679/74822826-6a857480-530e-11ea-8f54-32e1007c1cf2.png)

# Using this app

Using the app is a little cumbersome, but that's the price you pay in order to avoid integration with other tools. Make sure you carefully go through all steps, it's easy to miss some, and it won't work if you miss any:
1. Open your Facebook group's "Members" section
1. Above "All Members" there's a dropdown which defaults to "Default" – change it to "Join Date"
1. Scroll down until you reach the very end. Facebook loads the list in increments, this is tedious and takes time. I use the End key repeatedly, I find it easier than scrolling using the mouse wheel.
1. The last person you see should be the founder of the group. Even if the founder has left the group, we'll call the last person in the list the founder for the rest of this document.
1. Copy the founder's name and save it separately for future reference (you will need it later)
1. Click once somewhere in the founder's details, after his name; DO NOT CLICK ANYWHERE AGAIN UNTIL THIS DOCUMENT TELLS YOU OTHERWISE!
1. Notice that you are a monkey who takes orders from a document; take a couple of moments to contemplate on that
1. Scroll back up to the top
1. Holding the Shift key, click once somewhere inside the label "All members"
1. You should now have the entire list of members selected. Copy it and paste it in a new text document
1. Make sure the first thing in the text document is the name of the newest member in the group (your clipboard will contain some clutter at the top)
1. Save the text file, and save its full path separately for future reference
1. Open a command line console, go to the folder where you saved these sources, and [build the project](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build) – you should be able to simply execute `dotnet build` with no parameters
1. Execute the app using the following syntax: `dotnet run --no-build "founder" "file"`, where `founder` is the founder's name, and `file` is the full path and filename of the text file you saved before
1. If everything works out, you should get the list of members in the console, and a new file with the same path and filename as your text file, but with extension DOT.
1. Use a graph rendering tool to render your DOT file.

# How does it handle duplicate names?

Short answer: as best it can.

Facebook shows group additions like so:

```
Jane Doe
Added by John Doe yesterday
```

Jane Doe is listed with her full details (image, profile link, short info). If we parsed the HTML content instead of the plain text representation, we could distinguish between two accounts with the name Jane Doe. On the other hand, John Doe is only shown as plain text – even in the HTML source. There is nothing distinguishable between two people named John Doe when they *add* people to the group.

Since there's no way of knowing who added a certain person to the group, that means we don't really care to ever distinguish between duplicate accounts – it doesn't matter that you can distinguish between incoming nodes if you can't distinguish the outgoing nodes on an edge.

So the solution I implemented was this: all Jane Does are separate nodes, but all outgoing references use the first instance of Jane Doe that was encountered. The names in the labels are Jane Doe, Jane Doe@2, Jane Doe@3, etc. The code is not testing if it generates new duplicates by appending these indices because I was lazy.
