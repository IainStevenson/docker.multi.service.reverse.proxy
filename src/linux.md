# Notes on linux for newbies (like me)

To temporarily add additional tooling to the container 

Connect to the console (which is root) via the Containers panel.
```
	apt update
```


## To add IP tools

Then either;

```
	apt-get install iputils-ping
```

and/or;

```
	apt-get install net-tools 
```

## For privileged access

```
apt install sudo
```
After which you can perform actions requiring root access by prefixing other commands with ```sudo```

## For editing files
```
apt install nano
```

