db.createUser({
	user: "admin", // TODO: Configuration
	pwd: "admin", // TODO: Configuration
	roles: [
		{
			role: 'root',
			db: 'admin',
		},
	]
})

db.auth('admin', 'admin') // TODO: Configuration

db.createUser({
	user: 'storage', // TODO: Configuration
	pwd: 'storagepass', // TODO: Configuration
	roles: [
		{
			role: "readWrite",
			db: "myInfoApi"
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


db = db.getSiblingDB('myInfoIdentity')
db.createCollection('testusers')
var usersCol = db.getCollection('testusers')
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

db = db.getSiblingDB('myInfoApi')
db.createCollection('resources')
var resourcesCol = db.getCollection('resources')
resourcesCol.createIndex({ "Namespace": 1 }, { unique: false })
resourcesCol.createIndex({ "Metadata.$**": 1 }, { unique: false })


db = db.getSiblingDB('myInfoStore')
db.createCollection('resources')
db = db.getSiblingDB('myInfoSupport')
db.createCollection('resources')