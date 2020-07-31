# Enable all rules by default
all

# Extend line length, since each sentence should be on a separate line.
rule 'MD013', :line_length => 99999

# Allow multiple headers with same content.
exclude_rule 'MD024'

# The first line in a file does not have to be a top-level header.
# This is important for GitLab resource templates, etc.
exclude_rule 'MD002'
exclude_rule 'MD041'

# Allow trailing punctuation (e.g. question marks) in headers.
exclude_rule 'MD026'

# Allow in-line HTML
exclude_rule 'MD033'

# Nested lists should be indented with four spaces.
rule 'MD007', :indent => 4
