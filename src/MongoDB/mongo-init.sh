for keyval in $(grep -E '": [^\{]' /root/usersecrets/secrets.json | sed -e 's/: /=/' -e "s/\(\,\)$//"); 
do     
	echo "export $keyval" 
	eval export $keyval 
done
