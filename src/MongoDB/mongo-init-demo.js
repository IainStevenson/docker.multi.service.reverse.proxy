db.createUser({
	user: "admin",
	pwd: "admin",
	roles: [
		{
			role: 'root',
			db: 'admin',
		},
	]
})

db.auth('admin', 'admin')

db.createUser({
	user: 'storage',
	pwd: 'storagepass',
	roles: [
		{
			role: "readWrite",
			db: "myInfoAPI"
		},
		{
			role: "readWrite",
			db: "myInfoIdentity"
		},
		{
			role: "readWrite",
			db: "myInfoStore"
		},
		{
			role: "readWrite",
			db: "myInfoSupport"
		}
	],
});


db = db.getSiblingDB('myInfoAPI')
db.createCollection('resources')
db = db.getSiblingDB('myInfoIdentity')
db.createCollection('appusers')
var usersCol = db.getCollection('appusers')
usersCol.createIndex({ "SubjectId": 1 }, { unique: true })
usersCol.createIndex({ "ProviderName": 1, "ProviderSubjectId": 1 }, { unique: true })
usersCol.createIndex({ "Username": 1 })
db.createCollection('clients')
var clientsCol = db.getCollection('clients')
clientsCol.createIndex({ "ClientId": 1 }, { unique: true })
db.createCollection('apiresources')
var apiCol = db.getCollection('apiresources')
apiCol.createIndex({ "Name": 1 }, { unique: true })
db.createCollection('identityresources')
var idCol = db.getCollection('identityresources')
idCol.createIndex({ "Name": 1 }, { unique: true })