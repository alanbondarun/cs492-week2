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
				playGame(rooms[roomKey], roomKey);
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

function playGame(room, roomKey)
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

	var deck = [];
	setupDeck(deck);
	//deal cards
	p1.emit('deal', {cards : [deck.pop(), deck.pop()]});
	p2.emit('deal', {cards : [deck.pop(), deck.pop()]});

	p1.on('fold', function(){
		p2.emit('fold');
	});

	p2.on('fold', function(){
		p1.emit('fold');
	});

	var p1sum = 0;
	var p2sum = 0;
	var flopReceived = false;
	var turnReceived = false;
	var riverReceived = false;

	p1.on('bet', function(data){
		var p1bet = data[amount];
		cosole.log("player1 raised bet from " + p1sum + " to " + p1bet + ".");
		if (p1bet < p1sum)
		{
			//handle negative bet
		}
		p2.emit('bet', {amount: p1bet});
		p1sum = p1bet;
	});
	
	p2.on('bet', function(data){
		var p2bet = data[amount];
		cosole.log("player2 raised bet from " + p2sum + " to " + p2bet + ".");
		if (p2bet < p2sum)
		{
			//handle negative bet
		}
		p1.emit('bet', {amount: p2bet});
		p2sum = p2bet;
	});

	p1.on('flop', function()
	{
		if (!flopReceived)
		{
			io.sockets.in(roomKey).emit('flop', { cards: [ deck.pop(), deck.pop(), deck.pop() ] });
			flopReceived = true;
		}
	});


}

function setupDeck(deck)
{
	for (var i = 1; i < 14; i++)
	{
		deck.push({number: i, shape: "Heart"});
		deck.push({number: i, shape: "Spade"});
		deck.push({number: i, shape: "Club"});
		deck.push({number: i, shape: "Diamond"});		
	}
	shuffle(deck);
}

function shuffle(array)
{
	var m = array.length, t, i;
	while (m)
	{
		i = Math.floor(Math.random() * m--);
		t = array[m];
		array[m] = array[i];
		array[i] = t;
	}
	
}