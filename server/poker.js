var io = require('socket.io')({
	transports: ['websocket'],
});

io.attach(3000);

roomid = 0;

io.on('connection', function(socket){
	console.log("new connection: " + socket.id);
	socket.emit('msg', {message: 'you are connected to server'});
	
	socket.on('beep', function(){
		socket.emit('boop', {this : 'thisString', that: 'thatString'});
	});

	socket.on('match', function(){
		var rooms = io.sockets.adapter.rooms;
		console.log('iterating all rooms :');
		for (var roomKey in Object.keys(rooms))
		{
			console.log("room with key: " + roomKey);
			if (rooms[roomKey] == undefined)
			{
				continue;
			}
			if (Object.keys(rooms[roomKey]).length == 1)
			{
				socket.join(roomKey);
				io.sockets.in(roomKey).emit('match', {found: true});
				io.sockets.in(roomKey).emit('msg', {message: "match found in the room " + roomKey});
				roomid += 1;
				playGame(rooms[roomKey]);
				return;
			}
		}
		/*
		for (var room in rooms)
		{
			console.log("inspecting ");
			if (Object.keys(room).length == 1)
			{
				socket.join(room);
				io.sockets.in(room).emit('match', {found: true});
				io.sockets.in(room).emit('msg', {message: "match found in the room " + roomid.toString()});
				roomid += 1;
				playGame(room, roomid-1);
				return;
			}
		}
		*/
		socket.join(roomid.toString());
		socket.emit('match', {found: false});
		socket.emit('msg', {message: "waiting in the room " + roomid.toString()});
	});

	socket.on('disconnect', function(){
		var socketRooms = socket.rooms;
		for (var key in socketRooms)
		{
			socket.leave(key);
			io.sockets.in(key.emit('disconnect'));
			var clients = io.sockets.adapter.rooms.sockets;
			for (var client in clients)
			{
				client.leave(key);
			}
		}
	});
});

function playGame(room)
{
	var players = Object.keys(room);
	console.log(players);
	var id1 = players[0];
	var id2 = players[1];
	console.log(id1);
	console.log(id2);
	io.to(p1).emit('msg', {message : "you are player 1"});
	io.to(p2).emit('msg', {message : "you are player 2"});

	var p1 = io.sockets.connected[id1];
	var p2 = io.sockets.connected[id2];
	p1.emit('msg', {message : "again, you are player 1"});
	p2.emit('msg', {message : "again, you are player 2"});
}