package be.kdg.stemtest.DI.modules.fragmentModules

import be.kdg.stemtest.view.fragments.StatementFragment
import dagger.Module
import dagger.android.ContributesAndroidInjector

@Module
internal abstract class StatementFragmentModule {

    @ContributesAndroidInjector
    abstract fun bindStatementFragment(): StatementFragment
}