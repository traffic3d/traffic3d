## What does this MR do?

Add a description of your merge request here.

## Documentation checklist

- [ ] All documentation (except for user guides) is in [Markdown format](https://docs.gitlab.com/ee/user/markdown.html).
- [ ] Each sentence starts on a new line.
- [ ] Documentation passes [Markdown lint](https://github.com/markdownlint/markdownlint), with the config below:

```ruby
# Enable all rules by default
all

# Extend line length, since each sentence should be on a separate line.
rule 'MD013', :line_length => 99999

# Allow multiple headers with same content.
exclude_rule 'MD024'

# Allow trailing punctuation (e.g. question marks) in headers.
exclude_rule 'MD026'
```

/label ~Documentation
