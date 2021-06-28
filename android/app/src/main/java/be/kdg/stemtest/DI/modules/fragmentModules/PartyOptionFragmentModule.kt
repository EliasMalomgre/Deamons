package be.kdg.stemtest.DI.modules.fragmentModules

import be.kdg.stemtest.view.fragments.partygame.SelectParty
import dagger.Module
import dagger.android.ContributesAndroidInjector

@Module
internal abstract class PartyOptionFragmentModule {


    @ContributesAndroidInjector
    abstract fun bindPartyFragment(): SelectParty
}