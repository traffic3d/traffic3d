## Summary

Describe your proposed change, and why you think it will improve the code base.

[[_TOC_]]

---

## Test coverage

Which test cases cover the changes you have made in this merge request?

## SOLID principles

If your MR helps the code base to conform to one of the [SOLID principles](https://en.wikipedia.org/wiki/SOLID) of OOP, state which one(s):

- [ ] **Single responsibility principle** A class should only have a single responsibility, that is, only changes to one part of the software's specification should be able to affect the specification of the class.
- [ ] **Open–closed principle** Software entities ... should be open for extension, but closed for modification.
- [ ] **Liskov substitution principle** Objects in a program should be replaceable with instances of their subtypes without altering the correctness of that program.
- [ ] **Interface segregation principle** Many client-specific interfaces are better than one general-purpose interface.
- [ ] **Dependency inversion principle** One should depend upon abstractions, not concretions.

## Refactoring catalog

If your proposal implements one or more refactorings from [Fowler's catalogue](https://refactoring.com/catalog/), state which one(s):

- [ ] Change Function Declaration
- [ ] Change Reference to Value
- [ ] Change Value to Reference
- [ ] Collapse Hierarchy
- [ ] Combine Functions into Class
- [ ] Combine Functions into Transform
- [ ] Consolidate Conditional Expression
- [ ] Decompose Conditional
- [ ] Encapsulate Collection
- [ ] Encapsulate Record
- [ ] Encapsulate Variable
- [ ] Extract Class
- [ ] Extract Function / Method
- [ ] Extract Superclass
- [ ] Extract Variable
- [ ] Hide Delegate
- [ ] Inline Class
- [ ] Inline Function / Method
- [ ] Inline Variable
- [ ] Introduce Assertion
- [ ] Introduce Parameter Object
- [ ] Introduce Special Case
- [ ] Move Field
- [ ] Move Function / Method
- [ ] Move Statements into Function
- [ ] Move Statements to Callers
- [ ] Parameterize Function / Method
- [ ] Preserve Whole Object
- [ ] Pull Up Constructor Body
- [ ] Pull Up Field
- [ ] Pull Up Method
- [ ] Push Down Field
- [ ] Push Down Method
- [ ] Remove Dead Code
- [ ] Remove Flag Argument
- [ ] Remove Middle Man
- [ ] Remove Setting Method
- [ ] Remove Subclass
- [ ] Rename Field
- [ ] Rename Variable
- [ ] Replace Command with Function
- [ ] Replace Conditional with Polymorphism
- [ ] Replace Constructor with Factory Function / Method
- [ ] Replace Control Flag with Break
- [ ] Replace Derived Variable with Query
- [ ] Replace Error Code with Exception
- [ ] Replace Exception with Precheck / Test
- [ ] Replace Function with Command / Method with Method Object
- [ ] Replace Inline Code with Function Call
- [ ] Replace Loop with Pipeline
- [ ] Replace Magic Literal
- [ ] Replace Nested Conditional with Guard Clauses
- [ ] Replace Parameter with Query / Method
- [ ] Replace Primitive with Object
- [ ] Replace Query with Parameter
- [ ] Replace Subclass with Delegate
- [ ] Replace Superclass with Delegate / Replace Inheritance with Delegation
- [ ] Replace Temp with Query
- [ ] Replace Type Code with Subclasses
- [ ] Return Modified Value
- [ ] Separate Query from Modifier
- [ ] Slide Statements
- [ ] Split Loop
- [ ] Split Phase
- [ ] Split Variable
- [ ] Substitute Algorithm

## Design patterns

If your proposed refactoring will implement a [design pattern](https://en.wikipedia.org/wiki/Design_Patterns), state which one:

- [ ] **Abstract factory** groups object factories that have a common theme.
- [ ] **Builder** constructs complex objects by separating construction and representation.
- [ ] **Factory method** creates objects without specifying the exact class to create.
- [ ] **Prototype** creates objects by cloning an existing object.
- [ ] **Singleton** restricts object creation for a class to only one instance.
- [ ] **Adapter** allows classes with incompatible interfaces to work together by wrapping its own interface around that of an already existing class.
- [ ] **Bridge** decouples an abstraction from its implementation so that the two can vary independently.
- [ ] **Composite** composes zero-or-more similar objects so that they can be manipulated as one object.
- [ ] **Decorator** dynamically adds/overrides behaviour in an existing method of an object.
- [ ] **Facade** provides a simplified interface to a large body of code.
- [ ] **Flyweight** reduces the cost of creating and manipulating a large number of similar objects.
- [ ] **Proxy** provides a place-holder for another object to control access, reduce cost, and reduce complexity.
- [ ] **Chain of responsibility** delegates commands to a chain of processing objects.
- [ ] **Command** creates objects which encapsulate actions and parameters.
- [ ] **Interpreter** implements a specialized language.
- [ ] **Iterator** accesses the elements of an object sequentially without exposing its underlying representation.
- [ ] **Mediator** allows loose coupling between classes by being the only class that has detailed knowledge of their methods.
- [ ] **Memento** provides the ability to restore an object to its previous state (undo).
- [ ] **Observer** is a publish/subscribe pattern which allows a number of observer objects to see an event.
- [ ] **State** allows an object to alter its behaviour when its internal state changes.
- [ ] **Strategy** allows one of a family of algorithms to be selected on-the-fly at runtime.
- [ ] **Template method** defines the skeleton of an algorithm as an abstract class, allowing its subclasses to provide concrete behaviour.
- [ ] **Visitor** separates an algorithm from an object structure by moving the hierarchy of methods into one object.

/label ~Refactoring
