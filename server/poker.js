var io = require('socket.io')({
	transports: ['websocket'],
});

io.attach(3000);

roomid = 0;

io.on('connection', function(socket){
	console.log("new connection: " + socket.id);
	socket.emit('msg', {message: 'you are connected to server'});


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

			var room = io.sockets.adapter.rooms[key];

			for (var clientid in Object.keys(room))
			{
				io.sockets.connected[clientid].leave(key);
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

	var game = {};

	game.roomKey = roomKey;

	game.p1 = io.sockets.connected[id1];
	game.p2 = io.sockets.connected[id2];
	var p1 = game.p1;
	var p2 = game.p2;

	p1.emit('msg', {message : "again, you are player 1"});
	p2.emit('msg', {message : "again, you are player 2"});

	game.deck = [];
	setupDeck(game.deck);

	//initial card deal
	game.p1hand = [game.deck.pop(), game.deck.pop()];
	game.p2hand = [game.deck.pop(), game.deck.pop()];
	game.common = []
	p1.emit('deal', {cards: game.p1hand});
	p2.emit('deal', {cards: game.p2hand});

	game.p1sum = 0;
	game.p2sum = 0;
	game.flopReceived = false;
	game.turnReceived = false;
	game.riverReceived = false;
	game.resultReceived = false;

	p1.on('fold', function(){
		p2.emit('fold');
	});
	p2.on('fold', function(){
		p1.emit('fold');
	});
	
	p1.on('bet', function(data){
		var p1bet = data['amount'];
		console.log("player1 raised bet from " + game.p1sum + " to " + p1bet + ".");
		if (p1bet < game.p1sum)
		{
			console.log("error : new bet cannot be lower!");
			//handle negative bet
		}
		p2.emit('bet', {amount: p1bet});
		game.p1sum = p1bet;
	});
	p2.on('bet', function(data){
		var p2bet = data['amount'];
		console.log("player1 raised bet from " + game.p2sum + " to " + p2bet + ".");
		if (p2bet < game.p2sum)
		{
			console.log("error : new bet cannot be lower!");
			//handle negative bet
		}
		p1.emit('bet', {amount: p2bet});
		game.p2sum = p2bet;
	});

	p1.on('flop', function()
	{
		handleFlop(game);
	});
	p2.on('flop', function()
	{
		handleFlop(game);
	});
	p1.on('turn', function()
	{
		handleTurn(game);
	});
	p2.on('turn', function()
	{
		handleTurn(game);
	});
	p1.on('river', function()
	{
		handleRiver(game);
	});
	p2.on('river', function()
	{
		handleRiver(game);
	});
	p1.on('result', function()
	{
		handleResult(game);
	});
	p2.on('result', function()
	{
		handleResult(game);
	});
}

function handleBet(game, data)
{

}

function handleFlop(game)
{
	if (!game.flopReceived)
	{
		game.common.push(game.deck.pop());
		game.common.push(game.deck.pop());
		game.common.push(game.deck.pop());
		io.sockets.in(game.roomKey).emit('flop', { cards: game.common });
		game.flopReceived = true;
	}
}

function handleTurn(game)
{
	if (!game.turnReceived)
	{
		var card = game.deck.pop();
		game.common.push(card);
		io.sockets.in(game.roomKey).emit('turn', { cards: [card] });
		game.turnReceived = true;
	}
}

function handleRiver(game)
{
	if (!game.riverReceived)
	{
		var card = game.deck.pop();
		game.common.push(card);
		io.sockets.in(game.roomKey).emit('river', { cards: [card] });
		game.riverReceived = true;
		handleResult(game);
	}
}

function handleResult(game)
{
	if (!game.resultReceived)
	{
		var matchresult = p1wins(game);
		if (matchresult == 1)
		{
			game.p1.emit('result', {win : true, cards: game.p2hand});
			game.p2.emit('result', {win : false, cards: game.p1hand});
		}
		else if (matchresult == -1)
		{
			game.p1.emit('result', {win : false, cards: game.p2hand});
			game.p2.emit('result', {win : true, cards: game.p1hand});
		}
		else
		{
			game.p1.emit('result', {win : false, cards: game.p2hand});
			game.p2.emit('result', {win : false, cards: game.p1hand});	
		}
		game.resultReceived = true;
	}
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

function p1wins(game)
{
	var p1cards = game.p1hand.concat(game.common);
	var p2cards = game.p2hand.concat(game.common);
	var p1point = calculatePoint(p1cards, game.p1hand);
	var p2point = calculatePoint(p2cards, game.p2hand);
	console.log("p1's score : " + p1point.toString());
	console.log("p2's score : " + p2point.toString());
	if (p1point > p2point)
	{
		return 1;
	}
	if (p1point == p2point)
	{
		return 0;
	}
	return -1;
}

function calculatePoint(cards, hand)
{
	var topinhand = higher(hand[0].number, hand[1].number);

	topcard = {number : 0};
	if (findStraightFlush(cards, topcard))
	{
		return 800 + topcard.number*2 + topinhand;
	}
	if (isPair(4, cards, topcard))
	{
		return 700 + topcard.number*2 + topinhand;
	}
	if (isFullHouse(cards, topcard))
	{
		return 600 + topcard.number*2 + topinhand;
	}
	if (isFlush(cards))
	{
		return 500 + topinhand;
	}
	if (isStraight(cards, topcard))
	{
		return 400 + topcard.number*2 + topinhand;
	}
	if (isPair(3, cards, topcard))
	{
		return 300 + topcard.number*2 + topinhand;
	}
	if (isTwoPair(cards, topcard))
	{
		return 200 + topcard.number*2 + topinhand;
	}
	if (isPair(2, cards, topcard))
	{
		return 100 + topcard.number*2 + topinhand;
	}
	return topinhand;
}

function findStraightFlush(cards, topcard)
{
	if (isFlush(cards) && isStraight(cards, topcard))
	{
		return true;
	}
	return false;
}

function isFullHouse(cards, topcard)
{
	if (isPair(3, cards, topcard) && isPair(2, cards, {}))
	{
		return true;
	}
	return false;
}

function isFlush(cards)
{
	if (countShape(cards, "Heart") >= 5)
	{
		return true;
	}
	if (countShape(cards, "Spade") >= 5)
	{
		return true;
	}

	if (countShape(cards, "Club") >= 5)
	{
		return true;
	}
	if (countShape(cards, "Diamond") >= 5)
	{
		return true;
	}
	return false;
}

function countShape(cards, shape)
{
	var count = 0;
	for (var i = 0; i > cards.length; i++)
	{
		if (cards[i].shape == shape)
		{
			count++;
		}
	} 
	return count;
}

function isStraight(cards, topcard)
{
	for (var x = 14; x > 4; x--)
	{
		if (find(cards, x)
			&& find(cards, x-1)
			&& find(cards, x-2)
			&& find(cards, x-3)
			&& find(cards, x-4))
		{
			topcard.number = x;
			return true;
		}
	}
	return false;
}

function find(cards, num)
{
	var target = num;
	if (num == 14)
	{
		target = 1;
	}
	for (var i = 0; i < cards.length; i++)
	{
		if (cards[i].number == target)
			return true;
	}
	return false;
}

function isPair(num, cards, topcard)
{
	var found = false;
	var max = 0;
	for (var i = 0; i< cards.length; i++)
	{
		var x = cards[i].number;
		var count = 0
		for (var j = 0; j < cards.length; j++)
		{
			if (x == cards[j].number)
			{
				count += 1;
			}
		}
		if (count == num)
		{
			found = true;
			max = higher(x, max);
		}
	}
	if (found)
	{
		topcard.number = max;
		return true;
	}
	return false;
}

function higher(x, y)
{
	if (x == 1 || y == 1)
		return 14;
	return Math.max(x, y);
}

function isTwoPair(cards, topcard)
{
	var pairs = 0;
	var topNum = 0;
	
	if (!isPair(2, cards, topcard))
	{
		return false;
	}
	if (isPair)

	for (var i = 0; i< cards.length; i++)
	{
		var x = cards[i].number;
		if (x == topcard.number) {continue;}
		var count = 0
		for (var j = 0; j < cards.length; j++)
		{
			if (x == cards[j].number)
			{
				count += 1;
			}
		}
		if (count == 2)
		{
			return true;
		}
	}
	return false;
}