//using Gort.Commands;

//namespace UnitTests;

//public class CommandTests
//{
//    public CommandTests()
//    {
//        // Ensure fresh state before each test (xUnit creates a new instance per test)
//        Gort.Environment.Reset();
//        Gort.Environment.Variables.Clear();
//    }

//    //TODO: these are all wrong - keep them for now as a guide to why they are wrong.
//    //      replace in next version - major rewrite

//    [Fact]
//    public void CommandIf_TrueBranchesToNext()
//    {
//        // Arrange
//        var ifCmd = (CommandIf)CommandBase.GetCommand('{', 1)!;
//        ifCmd.Compile("1 == 1"); // condition that evaluates to true

//        var trueAssign = (CommandAssign)CommandBase.GetCommand('$', 2)!;
//        trueAssign.Compile("y 10");

//        var endif = (CommandEndif)CommandBase.GetCommand('}', 3)!;

//        // Link flow: if -> trueAssign ; if.Jump -> endif ; endif.Next -> null
//        ifCmd.Next = trueAssign;
//        ifCmd.Jump = endif;
//        endif.Next = null;

//        // Act: executing if should return the 'true' branch command
//        var next = ifCmd.Execute();

//        // Assert: next is the expected command and executing it assigns variable
//        Assert.Same(trueAssign, next);
//        next!.Execute();
//        Assert.Equal(10d, Gort.Environment.Variables["y"]!);
//    }

//    [Fact]
//    public void CommandIf_FalseJumpsToJumpNext()
//    {
//        // z 10
//        // { 1 == 2
//        //   z 99
//        // }

//        // Arrange
//        var assign10 = (CommandAssign)CommandBase.GetCommand('$', 2)!;
//        assign10.Compile("z 10");
        
//        var ifCmd = (CommandIf)CommandBase.GetCommand('{', 1)!;
//        ifCmd.Compile("1 == 2"); // condition false

//        var assign99 = (CommandAssign)CommandBase.GetCommand('$', 2)!;
//        assign99.Compile("z 99");

//        var endif = (CommandEndif)CommandBase.GetCommand('}', 3)!;
//        endif.Compile("");

//        // Act
//        var next = assign10.Execute();

//        // Assert: because condition is false we should jump to endif.Next (the elseAssign)
//        Assert.Null(next);

//        // and z hasn't been set to 99
//        Assert.Equal(10d, Gort.Environment.Variables["z"]);
//    }
//}
