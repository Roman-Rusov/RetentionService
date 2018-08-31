Backup Service generates several backup files during a day and places them on a disk. Design and implement a Retention Service that would be responsible for keeping proper amount of backup files according to a retention policy. A policy is defined by set of retention rules. Each rule should define how many files, that are older than an age specified, is allowed to keep on a disk. For example, a retention policy could be defined by the following set of rules:
* keep no files that are older than one month,
* keep one file that is older than two weeks,
* keep no more than five files that are older than ten days,
* keep no more than five files that are older than one week,
* keep no more than ten files that are older than three days.