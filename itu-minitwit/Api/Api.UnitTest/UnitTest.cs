using System.Data.Entity;
using System.Net;
using System.Text;
using System.Text.Json;
using Api.DataAccess.Models;
using Api.Services.Dto_s.FollowDTO_s;
using Api.Services.Dto_s.MessageDTO_s;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace Api.UnitTest;

public class UnitTest(InMemoryWebApplicationFactory fixture) : IClassFixture<InMemoryWebApplicationFactory>
{
    private readonly InMemoryWebApplicationFactory fixture = fixture;

    private readonly HttpClient client = fixture.CreateClient(new WebApplicationFactoryClientOptions
        { AllowAutoRedirect = true, HandleCookies = true });

    [Fact]
    public async Task GetLatest_FileIsEmpty_Minius1()
    {
        fixture.ResetDB();
        var response = await client.GetAsync("api/Latest");
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var latestValue = doc.RootElement.GetProperty("latest").GetInt32();
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Assert.Equal(-1, latestValue);
    }

    [Fact]
    public async Task GetLatest_ThereIsAValue_TheValue()
    {
        fixture.ResetDB();
        var dbContext = fixture.GetDbContext();
        var lastAction = new LatestProcessedSimAction { Latest = 230 };
        await dbContext.AddAsync(lastAction);
        await dbContext.SaveChangesAsync();

        var response = await client.GetAsync("api/Latest");
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var latestValue = doc.RootElement.GetProperty("latest").GetInt32();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Assert.Equal(lastAction.Latest, latestValue);
    }
    
    [Fact]
    public async Task GetMessages_Returns_Messages()
    {
        fixture.ResetDB();
        var context = fixture.GetDbContext();
        var user = new User { Username = "test2", Email = "test@test.com", PwHash = "23456" };
        var msg = new Message
            { AuthorId = 1, Text = "Hello from test", PubDate = (int)DateTimeOffset.Now.ToUnixTimeSeconds(), Flagged = 0};

        await context.Users.AddAsync(user); 
        await context.SaveChangesAsync();
        await context.Messages.AddAsync(msg);
        await context.SaveChangesAsync();


        var response = await client.GetAsync("/api/msgs");
        var json = await response.Content.ReadAsStringAsync();
        var messageResponse = JsonConvert.DeserializeObject<List<MessageDto>>(json);

        Assert.Equal(messageResponse!.First().Text, msg.Text);
    }
    
    [Fact]
    public async Task GetFilteredMessages_returnsFilteredMessages()
    {
        var context = fixture.GetDbContext();
        var user = new User { Username = "Man", Email = "Man@Man.com", PwHash = "23456" };
    
        var msg = new Message { AuthorId = 1, Text = "Hello from Man", PubDate = (int)DateTimeOffset.Now.ToUnixTimeSeconds() };
        var msg2 = new Message { AuthorId = 1, Text = "Hello again from Man", PubDate = (int)DateTimeOffset.Now.ToUnixTimeSeconds() };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        
        await context.Messages.AddAsync(msg);
        await context.Messages.AddAsync(msg2);

        await context.SaveChangesAsync();

        var response = await client.GetAsync("/api/msgs/Man");
        var json = await response.Content.ReadAsStringAsync();
        

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var messagesResponse = JsonConvert.DeserializeObject<MessagesResponse>(json);
            var messages = messagesResponse.Messages; // Access the messages

            foreach (var message in messages)
            {
                message.username.Should().Be(user.Username);
            }
        }
    }
    
    [Fact]
    public async Task GetEmptyFilteredMessages_returnsErrorResponse()
    {
        var context = fixture.GetDbContext();
        var user = new User { Username = "Man", Email = "Man@Man.com", PwHash = "23456" };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var response = await client.GetAsync("/api/msgs/Man");
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task PostMessage_CreatesMessageSuccessfully()
    {
        // Arrange
        var context = fixture.GetDbContext(); // This should return a properly set up in-memory context

        var user = new User { Username = "Man", Email = "Man@test.com", PwHash = "hashedpassword" };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var content = "Hello from Man";
    
        var messageDto = new CreateMessageDTO
        {
            Content = "Hello from Man"
        };

        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(messageDto),
            Encoding.UTF8,
            "application/json"
        );
    
        // Act
        var response = await client.PostAsync("/api/msgs/Man", jsonContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent); // Expecting 204 No Content

        var savedMessage = await Task.Run(() =>
            context.Messages.AsQueryable().SingleOrDefault(m => m.AuthorId == user.UserId)
        );

        savedMessage.Should().NotBeNull(); 
        savedMessage.Text.Should().Be(content);
    }

    
    public class MessageDto
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("pubDate")]
        public int pubDate { get; set; }
        [JsonProperty("username")]
        public string username { get; set; }
    }
    
    public class MessagesResponse
    {
        [JsonProperty("result")]
        public List<MessageDto> Messages { get; set; }
    }

    
    [Fact]
    public async Task FollowUser_FollowsItself_BadRequest()
    {
        // Arrange
        fixture.ResetDB();
        var dbContext = fixture.GetDbContext();

        // Prepare the DTO with the follow action
        var followDto = new FollowDTO
        {
            Follow = "test",  
        };

        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(followDto),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        var response = await client.PostAsync("/api/fllws/test", jsonContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var followRelation = dbContext.Followers
            .FirstOrDefault(f => f.WhoId == 1 && f.WhomId == 1); 

        Assert.Null(followRelation);
    }

    [Fact]
    public async Task FollowUser_FollowsUser_NoContent()
    {
        // Arrange
        fixture.ResetDB();
    
        var dbContext = fixture.GetDbContext();
    
        // Create users
        var user1 = new User { Username = "test", Email = "", PwHash = "" };
        var user2 = new User { Username = "test2", Email = "", PwHash = "" };
    
        dbContext.Users.Add(user1);
        dbContext.Users.Add(user2);
        await dbContext.SaveChangesAsync();
    
        // Act
        // Prepare the FollowDTO to follow "test2"
        var followDto = new FollowDTO
        {
            Follow = "test2",  // User "test" is trying to follow "test2"
        };

        // Serialize the FollowDTO into JSON
        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(followDto),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("/api/fllws/test", jsonContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var followRelation = dbContext.Followers
            .FirstOrDefault(f => f.WhoId == user1.UserId && f.WhomId == user2.UserId); // user1 follows user2
    
        Assert.NotNull(followRelation);
    }


    [Fact]
    public async Task UnfollowUser_UnfollowsItself_BadRequest()
    {
        // Arrange
        fixture.ResetDB();
        var dbContext = fixture.GetDbContext();

        // Create user "test"
        var user1 = new User { Username = "test", Email = "", PwHash = "" };
    
        dbContext.Users.Add(user1);
        await dbContext.SaveChangesAsync();
    
        // Act
        // Prepare the FollowDTO to unfollow "test" (user is trying to unfollow themselves)
        var followDto = new FollowDTO
        {
            Unfollow = "test",  // User "test" is trying to unfollow themselves
        };

        // Serialize the FollowDTO into JSON
        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(followDto),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("/api/fllws/test", jsonContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var user = dbContext.Users.FirstOrDefault(u => u.Username == "test");
        Assert.NotNull(user);  // User should still exist

        var followRelation = dbContext.Followers
            .FirstOrDefault(f => f.WhoId == user1.UserId && f.WhomId == user1.UserId); // User cannot unfollow themselves
    
        // Assert that no follow relation exists
        Assert.Null(followRelation);
    }

    
    [Fact]
    public async Task UnfollowUser_UnfollowsUser_NoContent()
    {
        // Arrange
        fixture.ResetDB();
        
        var dbContext = fixture.GetDbContext();
        var user1 = new User { UserId = 1, Username = "test", Email = "", PwHash = "" };
        var user2 = new User { UserId = 2, Username = "test2", Email = "", PwHash = "" };
        
        var followRelation = new Follower
        {
            WhoId = user1.UserId,
            WhomId = user2.UserId
        };

        dbContext.Users.Add(user1);
        dbContext.Users.Add(user2);
        dbContext.Followers.Add(followRelation);

        await dbContext.SaveChangesAsync();
        
        // Act
        var followDto = new FollowDTO
        {
            Unfollow = "test2"  // User1 is unfollowing "test2"
        };
        
        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(followDto), 
            Encoding.UTF8, 
            "application/json"
        );

        var response = await client.PostAsync("/api/fllws/test", jsonContent);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var user = dbContext.Users.FirstOrDefault(user => user.Username == "test2");

        // Assert
        Assert.NotNull(user);
    }
    
    [Fact]
    public async Task Getfollows_ReturnsFollows()
    {
        // Arrange
        fixture.ResetDB();
        
        var dbContext = fixture.GetDbContext();
        var user1 = new User { UserId = 1, Username = "test", Email = "", PwHash = "" };
        var user2 = new User { UserId = 2, Username = "test2", Email = "", PwHash = "" };

        var followRelation = new Follower
        {
            WhoId = user1.UserId,
            WhomId = user2.UserId
        };

        await dbContext.Users.AddAsync(user1);
        await dbContext.Users.AddAsync(user2);
        await dbContext.Followers.AddAsync(followRelation);

        await dbContext.SaveChangesAsync();
        
        // Act
        var response = await client.GetAsync("/api/fllws/test");
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var follows = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Assert.Equal(user2.Username, follows!["follows"].First());
    }

    #region Register
    
    [Fact]
    public async Task Register_UsernameValidation_StatusCode400()
    {
        var jsonPayload = System.Text.Json.JsonSerializer.Serialize(new
        {
            username = "",
            email = "test@test.com",
            pwd = "test123!"
        });
        
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/register", content);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var errorMessage = doc.RootElement.GetProperty("error_msg").GetString();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Assert.Equal("You have to enter a username", errorMessage);
    }

    [Fact]
    public async Task Register_EmailValidation_StatusCode400()
    {
        var jsonPayload = System.Text.Json.JsonSerializer.Serialize(new
        {
            username = "test",
            email = "test.com",
            pwd = "test123!"
        });

        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/register", content);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var errorMessage = doc.RootElement.GetProperty("error_msg").GetString();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Assert.Equal("You have to enter a valid email address", errorMessage);
    }

    [Fact]
    public async Task Register_PasswordValidation_StatusCode400()
    {
        var jsonPayload = System.Text.Json.JsonSerializer.Serialize(new
        {
            username = "test",
            email = "test@test.com",
            pwd = ""
        });

        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/register", content);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var errorMessage = doc.RootElement.GetProperty("error_msg").GetString();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Assert.Equal("You have to enter a password", errorMessage);
    }

    [Fact]
    public async Task Register_UsernameTaken_StatusCode400()
    {
        fixture.ResetDB();
        var dbContext = fixture.GetDbContext();
        User user = new User
        {
            Username = "test",
            Email = "test@test.com",
            PwHash = "test123!",
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        
        var jsonPayload = System.Text.Json.JsonSerializer.Serialize(new
        {
            username = "test",
            email = "test@test.com",
            pwd = "test123!"
        });
        
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/register", content);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var errorMessage = doc.RootElement.GetProperty("error_msg").GetString();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Assert.Equal("The username is already taken", errorMessage);
    }

    [Fact]
    public async Task Register_RegistersUser_StatusCode200()
    {
        var dbContext = fixture.GetDbContext();
        var jsonPayload = System.Text.Json.JsonSerializer.Serialize(new
        {
            username = "test",
            email = "test@test.com",
            pwd = "test123!"
        });
        
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/register", content);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var user = dbContext.Users.FirstOrDefault(user => user.Username == "test");
        Assert.NotNull(user);
    }

    #endregion
}